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

[<RequireQualifiedAccess>]
type EmailType =
    | Received
    | Sent

type Email =
    {
        Guid : Guid
        From : string
        To : string []
        Date : DateTime
        Subject : string
        Body : string
        Type : EmailType
        IsStared : bool
        IsTrashed : bool
        Tags : string []
        Ancestor : Guid option
    }

type DatabaseData =
    {
        Version : int
        Emails : Email []
    }

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

    static member Emails
        with get() : Lowdb.Lowdb =
            Database.Lowdb
                .get(!^"Emails")

    // static member Users
    //     with get() : Lowdb.Lowdb =
    //         Database.Lowdb
    //             .get(!^"Users")

    // static member GetUserById (userId: int) =
    //     Database.Users
    //         .find(createObj [ "Id" ==> userId])
    //         .value()
    //     |> function
    //        | null -> None
    //        | value -> unbox<User> value |> Some

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
                    Emails =
                        [|
                            {
                                Guid = Guid.NewGuid()
                                From = "emilie@mail.com"
                                To = [| "mangel.maxime@fulma.com" |]
                                Subject = "Fable - Newsletter #1"
                                Date = DateTime(2018, 11, 7, 9, 45, 33, DateTimeKind.Utc)
                                Body =
                                    """
Hello Kitty,

the main component used for creating the list of answer on [this page](https://mangelmaxime.github.io/fulma-demo/#question/0) is the media object.

Here are some useful ressources:

- [Fulma documentation](https://fulma.github.io/Fulma/#fulma/components/media)
- [Official Bulma documentation](https://fulma.github.io/Fulma/#fulma/components/media)
- [A great article on how it helps drasticaly reduce the number of lines](http://www.stubbornella.org/content/2010/06/25/the-media-object-saves-hundreds-of-lines-of-code)

Maxime
                                    """
                                Type = EmailType.Received
                                IsStared = false
                                IsTrashed = false
                                Tags = [| |]
                                Ancestor = None
                            }
                            {
                                Guid = Guid.Parse("3c11ef7f-8ace-430d-a880-aad798c11367")
                                From = "kitty@mail.com"
                                To = [| "mangel.maxime@fulma.com" |]
                                Subject = "Where can I found the documentation of Fulma?"
                                Date = DateTime(2019, 1, 10, 9, 45, 33, DateTimeKind.Utc)
                                Body =
                                    """
Hello Maxime,

I just found the [Fulma demo](https://mangelmaxime.github.io/fulma-demo/) application and would like to know which components has been used to create the list of answers.

Can you please point me in the right direction?

Kitty
                                    """
                                Type = EmailType.Received
                                IsStared = false
                                IsTrashed = false
                                Tags = [| |]
                                Ancestor = None
                            }
                            {
                                Guid = Guid.Parse("30182082-9be1-4da0-834e-bef3d8234ee8")
                                From = "mangel.maxime@fulma.com"
                                To = [| "kitty@mail.com" |]
                                Subject = "Re: Where can I found the documentation of Fulma?"
                                Date = DateTime(2019, 1, 10, 10, 45, 33, DateTimeKind.Utc)
                                Body =
                                    """
Hello Kitty,

the main component used for creating the list of answer on [this page](https://mangelmaxime.github.io/fulma-demo/#question/0) is the media object.

Here are some useful ressources:

- [Fulma documentation](https://fulma.github.io/Fulma/#fulma/components/media)
- [Official Bulma documentation](https://fulma.github.io/Fulma/#fulma/components/media)
- [A great article on how it helps drasticaly reduce the number of lines](http://www.stubbornella.org/content/2010/06/25/the-media-object-saves-hundreds-of-lines-of-code)

Maxime
                                    """
                                Type = EmailType.Sent
                                IsStared = false
                                IsTrashed = false
                                Tags = [| |]
                                Ancestor = Some (Guid.Parse("3c11ef7f-8ace-430d-a880-aad798c11367"))
                            }
                            {
                                Guid = Guid.Parse("93d94890-b7bc-4f50-ad8d-67def5fc1d82")
                                From = "mangel.maxime@fulma.com"
                                To = [| "kitty@mail.com" |]
                                Subject = "Re: Re: Where can I found the documentation of Fulma?"
                                Date = DateTime(2019, 1, 10, 10, 45, 33, DateTimeKind.Utc)
                                Body =
                                    """
Awesome,

thank you.
                                    """
                                Type = EmailType.Received
                                IsStared = false
                                IsTrashed = false
                                Tags = [| |]
                                Ancestor = Some (Guid.Parse("30182082-9be1-4da0-834e-bef3d8234ee8"))
                            }
                        |]
                }
            ).write()
        Logger.debug "Database restored"
