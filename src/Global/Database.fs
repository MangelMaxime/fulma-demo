[<AutoOpen>]
module Database

open Fable.Import
open Fable.Core.JsInterop
open System

/// Shared types between the Client and the Database part

// If we update the database content or structure we need to increment this value
let [<Literal>] CurrentVersion = 6

type User =
    { Id : int
      Firstname: string
      Surname: string
      Avatar : string }

type Answer =
    { CreatedAt : DateTime
      AuthorId : User
      Content : string }

type Question =
    { Id : int
      AuthorId : int
      Title : string
      Description : string
      CreatedAt : DateTime
      Answers : Answer [] }

type DatabaseData =
    { Version : int
      Questions : Question []
      Users : User [] }

/// Database helpers

let adapterOptions = jsOptions<Lowdb.AdapterOptions>(fun o ->
    o.serialize <- Some toJson
    o.deserialize <- ofJson<DatabaseData> >> box |> Some
)

let private adapter = Lowdb.LocalStorageAdapter("database", adapterOptions)

let mutable private dbInstance : Lowdb.Lowdb option = Option.None

type Database =
    static member Lowdb
        with get() : Lowdb.Lowdb =
            if dbInstance.IsNone then
                dbInstance <- Lowdb.Lowdb(adapter) |> Some

            dbInstance.Value

    static member Questions
        with get() : Lowdb.Lowdb =
            Database.Lowdb
                .get(!^"Questions")

    static member Users
        with get() : Lowdb.Lowdb =
            Database.Lowdb
                .get(!^"Users")

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
        Browser.localStorage.removeItem("database")
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

                             """
                         Answers = [||]
                         CreatedAt = DateTime.Parse "2017-09-14T17:44:28.103Z" }
                       { Id = 1
                         AuthorId = 0
                         Title = "What is the average wing speed of an unladen swallow?"
                         Description =
                             """

                             """
                         Answers = [||]
                         CreatedAt = DateTime.Parse "2017-09-12T09:27:28.103Z" } |]
                  Users =
                    [| { Id = 0
                         Firstname = "Maxime"
                         Surname = "Mangel"
                         Avatar = "maxime_mangel.png" }
                       { Id = 1
                         Firstname = "Robin"
                         Surname = "Munn"
                         Avatar = "robin_munn.png" } |]
                }
            ).write()
        Logger.debug "Database restored"
