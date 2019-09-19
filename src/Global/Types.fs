module Types

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
