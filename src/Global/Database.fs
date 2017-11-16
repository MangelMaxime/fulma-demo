[<RequireQualifiedAccess>]
module Database

open Fable.Import
open Fable.Core.JsInterop
open System

/// Shared types between the Client and the Database part

// If we update the database content or structure we need to increment this value
let [<Literal>] CurrentVersion = 7

type User =
    { Id : int
      Firstname: string
      Surname: string
      Avatar : string }

type Answer =
    { Id : int
      CreatedAt : DateTime
      AuthorId : int
      Content : string
      Score : int }

type Question =
    { Id : int
      AuthorId : int
      Title : string
      Description : string
      CreatedAt : DateTime
      Answers : Answer array }

type DatabaseData =
    { Version : int
      Questions : Question array
      Users : User array }

/// Database helpers

let adapterOptions = jsOptions<Lowdb.AdapterOptions>(fun o ->
    o.serialize <- Some toJson
    o.deserialize <- ofJson<DatabaseData> >> box |> Some
)

let mutable private dbInstance : Lowdb.Lowdb option = None

type Engine =
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
            Engine.Lowdb
                .get(!^"Questions")

    static member Users
        with get() : Lowdb.Lowdb =
            Engine.Lowdb
                .get(!^"Users")

    static member GetUserById (userId: int) =
        Engine.Users
            .find(createObj [ "Id" ==> userId])
            .value()
        |> function
           | null -> None
           | value -> unbox<User> value |> Some

    static member Version
        with get() : int =
            Engine.Lowdb
                .get(!^"Version")
                .value()

    static member Init () =
        Logger.debug "Init database"
        try
            Logger.debugfn "Database.Version: %i" Engine.Version
            Logger.debugfn "CurrentVersion: %i" CurrentVersion
            if Engine.Version <> CurrentVersion then
                Logger.debug "Migration detected"
                Engine.Restore()
        with
            | _ ->
                Logger.debug "Failed to parse database from storage"
                Engine.Restore()

    static member Restore () =
        Logger.debug "Restore the database"
        Browser.localStorage.removeItem("database")
        dbInstance <- None
        Engine.Default()

    static member Default () =
        Engine.Lowdb
            .defaults(
                { Version = CurrentVersion
                  Questions =
                    [| { Id = 0
                         AuthorId = 1
                         Title = "What is the average wing speed of an unladen swallow?"
                         Description =
                             """
Hello, yesterday I saw a flight of swallows and was wondering what their **average wing speed** is ?

If you know the answer please share it.
                             """
                         Answers =
                            [| { Id = 0
                                 CreatedAt = DateTime.Parse "2017-09-14T19:57:33.103Z"
                                 AuthorId = 0
                                 Score = 2
                                 Content =
                                    """
> What do you mean, an African or European Swallow ?
>
> Monty Python’s: The Holy Grail

Ok I must admit, I use google to search the question and found a post explaining the reference :).

I thought you was asking it seriously well done.
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
                         Title = "Why did you create Fable ?"
                         Description =
                             """
Hello Alfonso,

I wanted to know why did you create Fable. Did you always planned to use F# ? Or was you thinking to others languages ?
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
                         Firstname = "Guess"
                         Surname = ""
                         Avatar = "guess.png" }
                          |]
                }
            ).write()
        Logger.debug "Database restored"
