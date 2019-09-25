[<RequireQualifiedAccess>]
module Router

open Browser
open Fable.React.Props
open Elmish.Navigation
open Elmish.UrlParser

[<RequireQualifiedAccess>]
type MailboxRoute =
    | Inbox of int option
    | Sent of int option
    | Archive of int option
    | Stared of int option
    | Trash of int option

[<RequireQualifiedAccess>]
type SettingsRoute =
    | Folders
    | Labels

[<RequireQualifiedAccess>]
type SessionRoute =
    | Restore
    | Logout

type Route =
    | Mailbox of MailboxRoute
    | Settings of SettingsRoute
    | Login
    | Session of SessionRoute

let private segment (pathA : string) (pathB : string) =
    pathA + "/" + pathB

let private mailboxRouteToHash (baseRoute : string) (pageRank : int option) =
    let parameters =
        match pageRank with
        | Some pageRank -> "?page=" + string pageRank
        | None -> ""

    baseRoute + parameters

let private toHash page =
    match page with
    | Mailbox mailboxRoute ->
        match mailboxRoute with
        | MailboxRoute.Inbox pageRank ->
            mailboxRouteToHash "inbox" pageRank

        | MailboxRoute.Archive pageRank ->
            mailboxRouteToHash "archive" pageRank

        | MailboxRoute.Sent pageRank ->
            mailboxRouteToHash "sent" pageRank

        | MailboxRoute.Stared pageRank ->
            mailboxRouteToHash "stared" pageRank

        | MailboxRoute.Trash pageRank ->
            mailboxRouteToHash "trash" pageRank

        |> segment "mailbox"

    | Settings settingsRoute ->
        match settingsRoute with
        | SettingsRoute.Folders ->
            "folders"
        | SettingsRoute.Labels ->
            "labels"
        |> segment "settings"

    | Session sessionRoute ->
        match sessionRoute with
        | SessionRoute.Restore ->
            "restore"
        | SessionRoute.Logout ->
            "logout"

        |> segment "session"

    | Login ->
        "login"

    |> segment "#"

let pageParser: Parser<Route -> Route, Route> =
    oneOf
        [
            map (MailboxRoute.Inbox >> Mailbox) (s "mailbox" </> s "inbox" <?> intParam "page")
            map (MailboxRoute.Sent >> Mailbox) (s "mailbox" </> s "sent" <?> intParam "page")
            map (MailboxRoute.Stared >> Mailbox) (s "mailbox" </> s "stared" <?> intParam "page")
            map (MailboxRoute.Trash >> Mailbox) (s "mailbox" </> s "trash" <?> intParam "page")
            map (MailboxRoute.Archive >> Mailbox) (s "mailbox" </> s "archive" <?> intParam "page")

            map (SettingsRoute.Folders |> Settings) (s "settings" </> s "folders")
            map (SettingsRoute.Labels |> Settings) (s "settings" </> s "labels")

            map Login (s "login")

            map (SessionRoute.Restore |> Session) (s "session" </> s "restore")
            map (SessionRoute.Logout |> Session) (s "session" </> s "logout")

            map (SessionRoute.Restore |> Session) top
        ]

let href route =
    Href (toHash route)

let modifyUrl route =
    route |> toHash |> Navigation.modifyUrl

let newUrl route =
    route |> toHash |> Navigation.newUrl

let modifyLocation route =
    window.location.href <- toHash route
