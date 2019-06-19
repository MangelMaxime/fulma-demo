[<AutoOpen>]
module Database

open System
open Browser
open Fable.Core
open Fable.Core.JsInterop
open Thoth.Json

/// Shared types between the Client and the Database part

/// If we update the database content or structure we need to increment this value
let [<Literal>] CurrentVersion = 0

type EmailType =
    | Received
    | Sent

type Email =
    {
        From : string list
        To : string list
        Subject : string
        Body : string
        Type : EmailType
        IsStared : bool
        IsTrashed : bool
        Tags : string list
    }

type DatabaseData =
    { Version : int
      Emails : Email list }

/// Database helpers

let adapterOptions = jsOptions<Lowdb.AdapterOptions>(fun o ->
    o.serialize <- Some(fun value ->
        let encoder = Encode.Auto.generateEncoderCached<DatabaseData>()
        let json =
            try
                encoder !!value
            // If the database is empty we received the object: {}
            // We catch the error to gracefully init the database
            with
                | _ -> Encode.object []

        Encode.toString 0 json
    )

    o.deserialize <- Some(fun (data:string) ->
        let decoder = Decode.Auto.generateDecoderCached<DatabaseData>()
        Decode.unsafeFromString decoder data
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
                {
                    Version = CurrentVersion
                    Emails = []
                }
            ).write()
        Logger.debug "Database restored"
