[<RequireQualifiedAccess>]
module Router

open Browser
open Fable.React.Props
open Elmish.Navigation
open Elmish.UrlParser

[<RequireQualifiedAccess>]
type MailboxPage =
    | Inbox
    | Sent
    | Stared
    | Trash

type Page =
    | Mailbox of MailboxPage

let private segment (pathA : string) (pathB : string) =
    pathA + "/" + pathB

let private toHash page =
    match page with
    | Mailbox mailboxPage ->
        match mailboxPage with
        | MailboxPage.Inbox ->
            "inbox"
        | MailboxPage.Sent ->
            "sent"
        | MailboxPage.Stared ->
            "stared"
        | MailboxPage.Trash ->
            "trash"
        |> segment "mailbox/"
    |> segment "#/"

let pageParser: Parser<Page->Page,Page> =
    oneOf
        [ map (MailboxPage.Inbox |> Mailbox) (s "mailbox" </> s "inbox")
        ; map (MailboxPage.Sent |> Mailbox) (s "mailbox" </> s "sent")
        ; map (MailboxPage.Stared |> Mailbox) (s "mailbox" </> s "stared")
        ; map (MailboxPage.Trash |> Mailbox) (s "mailbox" </> s "trash")
        ; map (MailboxPage.Inbox |> Mailbox) top
        ]

let href route =
    Href (toHash route)

let modifyUrl route =
    route |> toHash |> Navigation.modifyUrl

let newUrl route =
    route |> toHash |> Navigation.newUrl

let modifyLocation route =
    window.location.href <- toHash route
