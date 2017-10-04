[<AutoOpen>]
module Database

open Fable.Import
open Fable.Core.JsInterop

/// Shared types between the Client and the Database part

// If we update the database content or structure we need to increment this value
let [<Literal>] CurrentVersion = 1

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
      CreatedAt : string }

type DatabaseData =
    { Version : int
      Questions : Question [] }

/// Database helpers

let private adapter = Lowdb.LocalStorageAdapter("database")

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
        if Database.Version <> CurrentVersion then
            Browser.localStorage.removeItem("database")
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
                         CreatedAt = "" }
                       { Id = 1
                         Author =
                             { Id = 0
                               Firstname = "Alfonso"
                               Surname = "Garciacaro"
                               Avatar = "alfonso_garciacaro.png" }
                         Title = "What is the average wing speed of an unladen swallow?"
                         Description =
                             """

                             """
                         CreatedAt = "" }
                       { Id = 2
                         Author =
                             { Id = 0
                               Firstname = "Robin"
                               Surname = "Munn"
                               Avatar = "robin_munn.png" }
                         Title = "What is the average wing speed of an unladen swallow?"
                         Description =
                             """

                             """
                         CreatedAt = "" } |]
                }
            ).write()
