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

    static member Fake =
        {
            UserId = Guid.Parse("f7549690-05aa-4ae9-95f7-de486979d62f")
            Firstname = "Fake"
            Surname = "Session"
            Email = "mangel.maxime@fulma.com"
            SimpleJWT = DateTime.UtcNow.AddDays(5.)
            RefreshToken = ""
        }

module Email =

    [<RequireQualifiedAccessAttribute>]
    type Category =
        | Inbox
        | Sent
        | Archive
        | Stared
        | Trash
        | Folder of string
        | Tag of string
