[<RequireQualifiedAccess>]
module Router

open Browser
open Fable.React.Props
open Elmish.Navigation
open Elmish.UrlParser

[<RequireQualifiedAccess>]
type MailboxRoute =
    | Inbox of int option
    | Sent
    | Stared
    | Trash

[<RequireQualifiedAccess>]
type SettingsRoute =
    | Folders
    | Labels

type Route =
    | Mailbox of MailboxRoute
    | Settings of SettingsRoute

let private segment (pathA : string) (pathB : string) =
    pathA + "/" + pathB

let private toHash page =
    match page with
    | Mailbox mailboxRoute ->
        match mailboxRoute with
        | MailboxRoute.Inbox pageRank ->
            let parameters =
                match pageRank with
                | Some pageRank -> "?page=" + string pageRank
                | None -> ""

            "inbox" + parameters
        | MailboxRoute.Sent ->
            "sent"
        | MailboxRoute.Stared ->
            "stared"
        | MailboxRoute.Trash ->
            "trash"
        |> segment "mailbox"

    | Settings settingsRoute ->
        match settingsRoute with
        | SettingsRoute.Folders ->
            "folders"
        | SettingsRoute.Labels ->
            "labels"
        |> segment "settings"

    |> segment "#"

let pageParser: Parser<Route -> Route, Route> =
    oneOf
        [
            map (MailboxRoute.Inbox >> Mailbox) (s "mailbox" </> s "inbox" <?> intParam "page")
            map (MailboxRoute.Sent |> Mailbox) (s "mailbox" </> s "sent")
            map (MailboxRoute.Stared |> Mailbox) (s "mailbox" </> s "stared")
            map (MailboxRoute.Trash |> Mailbox) (s "mailbox" </> s "trash")

            map (SettingsRoute.Folders |> Settings) (s "settings" </> s "folders")
            map (SettingsRoute.Labels |> Settings) (s "settings" </> s "labels")

            // Default page of the application
            map (None |> MailboxRoute.Inbox |> Mailbox) top
        ]

let href route =
    Href (toHash route)

let modifyUrl route =
    route |> toHash |> Navigation.modifyUrl

let newUrl route =
    route |> toHash |> Navigation.newUrl

let modifyLocation route =
    window.location.href <- toHash route
