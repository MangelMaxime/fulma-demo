[<AutoOpen>]
module Database

open Fable.Import
open Fable.Core.JsInterop
open System

/// Shared types between the Client and the Database part

// If we update the database content or structure we need to increment this value
let [<Literal>] CurrentVersion = 4

type Author =
    { Id : int
      Firstname: string
      Surname: string
      Avatar : string }

type Question =
    { Id : int
      Author : Author
      Title : string
      Description : string
      CreatedAt : DateTime }

type DatabaseData =
    { Version : int
      Questions : Question [] }

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

    static member Version
        with get() : int =
            Database.Lowdb
                .get(!^"Version")
                .value()

    static member Init () =
        Logger.debug "Init database"
        Logger.debugfn "Database.Version: %i" Database.Version
        Logger.debugfn "CurrentVersion: %i" CurrentVersion
        try
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
                         Author =
                             { Id = 0
                               Firstname = "Maxime"
                               Surname = "Mangel"
                               Avatar = "maxime_mangel.png" }
                         Title = "What is the average wing speed of an unladen swallow?"
                         Description =
                             """

                             """
                         CreatedAt = DateTime.Parse "2017-09-14T17:44:28.103Z" }
                       { Id = 1
                         Author =
                             { Id = 0
                               Firstname = "Robin"
                               Surname = "Munn"
                               Avatar = "robin_munn.png" }
                         Title = "What is the average wing speed of an unladen swallow?"
                         Description =
                             """

                             """
                         CreatedAt = DateTime.Parse "2017-09-12T09:27:28.103Z" } |]
                }
            ).write()
        Logger.debug "Database restored"
