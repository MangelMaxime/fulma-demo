module Types

open System

type JWT = DateTime

type Session =
    {
        UserId : Guid
        Firstname : string
        Surname : string
        Email : string
        SimpleJWT : JWT
        RefreshToken : string
    }

module Email =

    [<RequireQualifiedAccessAttribute>]
    type Category =
        | Inbox
        | Sent
        | Archive
        | Starred
        | Trash
        | Folder of string
        | Tag of string
