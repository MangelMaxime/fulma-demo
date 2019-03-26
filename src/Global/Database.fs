[<AutoOpen>]
module Database

open System
open Browser
open Fable.Core
open Fable.Core.JsInterop
open Thoth.Json

/// Shared types between the Client and the Database part

/// If we update the database content or structure we need to increment this value
let [<Literal>] CurrentVersion = 9


module Encode =
    let datetime (date : DateTime) =
        date.ToString("O") |> Encode.string

type User =
    { Id : int
      Firstname: string
      Surname: string
      Avatar : string }

    static member Decoder =
        Decode.object (fun get ->
            { Id        = get.Required.Field "id" Decode.int
              Firstname = get.Required.Field "firstname" Decode.string
              Surname   = get.Required.Field "surname" Decode.string
              Avatar    = get.Required.Field "avatar" Decode.string } : User)

    static member Encoder user =
        Encode.object [
            "id", Encode.int user.Id
            "firstname", Encode.string user.Firstname
            "surname", Encode.string user.Surname
            "avatar", Encode.string user.Avatar
        ]

type Answer =
    { Id : int
      CreatedAt : DateTime
      AuthorId : int
      Content : string
      Score : int }

    static member Decoder =
        Decode.object (fun get ->
            { Id        = get.Required.Field "id" Decode.int
              CreatedAt = get.Required.Field "created_at" Decode.datetime
              AuthorId  = get.Required.Field "author_id" Decode.int
              Content   = get.Required.Field "content" Decode.string
              Score     = get.Required.Field "score" Decode.int } : Answer)

    static member Encoder answer =
        Encode.object [
            "id", Encode.int answer.Id
            "created_at", Encode.datetime answer.CreatedAt
            "author_id", Encode.int answer.AuthorId
            "content", Encode.string answer.Content
            "score", Encode.int answer.Score
        ]

type Question =
    { Id : int
      AuthorId : int
      Title : string
      Description : string
      CreatedAt : DateTime
      Answers : Answer [] }

    static member Decoder =
        Decode.object (fun get ->
            { Id          = get.Required.Field "id" Decode.int
              AuthorId    = get.Required.Field "author_id" Decode.int
              Title       = get.Required.Field "title" Decode.string
              Description = get.Required.Field "description" Decode.string
              CreatedAt   = get.Required.Field "created_at" Decode.datetime
              Answers     = get.Required.Field "answers" (Decode.array Answer.Decoder) } : Question)

    static member Encoder question =
        Encode.object [
            "id", Encode.int question.Id
            "author_id", Encode.int question.AuthorId
            "title", Encode.string question.Title
            "description", Encode.string question.Description
            "created_at", Encode.datetime question.CreatedAt
            "answers", Encode.array (question.Answers |> Array.map Answer.Encoder)
        ]

type DatabaseData =
    { Version : int
      Questions : Question []
      Users : User [] }

    static member Decoder =
        Decode.object (fun get ->
            { Version   = get.Required.Field "version" Decode.int
              Questions = get.Required.Field "questions" (Decode.array Question.Decoder)
              Users     = get.Required.Field "users" (Decode.array User.Decoder) } : DatabaseData)

    static member Encoder databaseData =
        try
            Encode.object [
                "version", Encode.int databaseData.Version
                "questions", Encode.array (databaseData.Questions |> Array.map Question.Encoder)
                "users", Encode.array (databaseData.Users |> Array.map User.Encoder)
            ]
        // If the database is empty we received the object: {}
        // We catch the error to gracefully init the database
        with
            | _ -> Encode.object []

/// Database helpers

let adapterOptions = jsOptions<Lowdb.AdapterOptions>(fun o ->
    o.serialize <- Some(DatabaseData.Encoder >> Encode.toString 0)

    o.deserialize <- Some(fun (data:string) ->
        match Decode.fromString DatabaseData.Decoder data with
        | Ok databaseData -> databaseData
        | Error msg -> failwith msg
    )
)

let mutable private dbInstance : Lowdb.Lowdb option = Option.None

type Database =
    static member Lowdb
        with get() : Lowdb.Lowdb =
            if dbInstance.IsNone then
                dbInstance <-
                    Lowdb.LocalStorageAdapter("database", adapterOptions)
                    |> Lowdb.Lowdb
                    |> Some

            dbInstance.Value

    static member Questions
        with get() : Lowdb.Lowdb =
            Database.Lowdb
                .get(!^"Questions")

    static member Users
        with get() : Lowdb.Lowdb =
            Database.Lowdb
                .get(!^"Users")

    static member GetUserById (userId: int) =
        Database.Users
            .find(createObj [ "Id" ==> userId])
            .value()
        |> function
           | null -> None
           | value -> unbox<User> value |> Some

    static member Version
        with get() : int =
            Database.Lowdb
                .get(!^"Version")
                .value()

    static member Init () =
        Logger.debug "Init database"
        try
            Logger.debugfn "Database.Version: %i" Database.Version
            Logger.debugfn "CurrentVersion: %i" CurrentVersion
            if Database.Version <> CurrentVersion then
                Logger.debug "Migration detected"
                Database.Restore()
        with
            | _ ->
                Logger.debug "Failed to parse database from storage"
                Database.Restore()

    static member Restore () =
        Logger.debug "Restore the database"
        localStorage.removeItem("database")
        dbInstance <- None
        Database.Default()

    static member Default () =
        Database.Lowdb
            .defaults(
                { Version = CurrentVersion
                  Questions =
                    [| { Id = 0
                         AuthorId = 1
                         Title = "What is the average wing speed of an unladen swallow?"
                         Description =
                             """
Hello, yesterday I saw a flight of swallows and was wondering what their **average wing speed** is?

If you know the answer please share it.
                             """
                         Answers =
                            [| { Id = 0
                                 CreatedAt = DateTime.Parse "2017-09-14T19:57:33.103Z"
                                 AuthorId = 0
                                 Score = 2
                                 Content =
                                    """
> What do you mean, an African or European Swallow?
>
> Monty Python’s: The Holy Grail

Ok I must admit, I use google to search the question and found a post explaining the reference :).

I thought you were asking it seriously, well done.
                                    """ }
                               { Id = 1
                                 CreatedAt = DateTime.Parse "2017-09-14T20:07:27.103Z"
                                 AuthorId = 2
                                 Score = 1
                                 Content =
                                    """
Maxime,

I believe you found [this blog post](http://www.saratoga.com/how-should-i-know/2013/07/what-is-the-average-air-speed-velocity-of-a-laden-swallow/).

And so Robin, the conclusion of the post is:

> In the end, it’s concluded that the airspeed velocity of a (European) unladen swallow is about 24 miles per hour or 11 meters per second.
                                    """ }
                            |]
                         CreatedAt = DateTime.Parse "2017-09-14T17:44:28.103Z" }
                       { Id = 1
                         AuthorId = 0
                         Title = "Why did you create Fable?"
                         Description =
                             """
Hello Alfonso,

I wanted to know why you created Fable. Did you always plan to use F#? Or were you thinking in others languages?
                             """
                         Answers = [| |]
                         CreatedAt = DateTime.Parse "2017-09-12T09:27:28.103Z" } |]
                  Users =
                    [| { Id = 0
                         Firstname = "Maxime"
                         Surname = "Mangel"
                         Avatar = "maxime_mangel.png" }
                       { Id = 1
                         Firstname = "Robin"
                         Surname = "Munn"
                         Avatar = "robin_munn.png" }
                       { Id = 2
                         Firstname = "Alfonso"
                         Surname = "Garciacaro"
                         Avatar = "alfonso_garciacaro.png" }
                       { Id = 3
                         Firstname = "Guest"
                         Surname = ""
                         Avatar = "guest.png" }
                          |]
                }
            ).write()
        Logger.debug "Database restored"
