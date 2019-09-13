module Mailbox

open Fulma
open Elmish
open Fable.FontAwesome
open Fable.React
open Fable.React.Props

[<RequireQualifiedAccess>]
type Page =
    | Inbox of Mailbox.Inbox.Model

type Model =
    {
        Page : Page
    }

type Msg =
    | InboxMsg of Mailbox.Inbox.Msg

let init (route : Router.MailboxRoute) =
    match route with
    | Router.MailboxRoute.Inbox ->
        let (inboxModel, inboxCmd) = Mailbox.Inbox.init ()
        {
            Page = Page.Inbox inboxModel
        }
        , Cmd.map InboxMsg inboxCmd

    | _ ->
        failwithf "Route not supported yet: %A" route

let update (msg  : Msg) (model : Model) =
    match msg with
    | InboxMsg inboxMsg ->
        match model.Page with
        | Page.Inbox inboxModel ->
            let (inboxModel, inboxCmd) = Mailbox.Inbox.update inboxMsg inboxModel
            { model with
                Page = Page.Inbox inboxModel
            }
            , Cmd.map InboxMsg inboxCmd


let private item txt icon isActive =
    Menu.Item.li [ Menu.Item.IsActive isActive ]
        [ Icon.icon [ ]
            [ Fa.i [ icon ]
                [ ]
            ]
          str txt
        ]

let private renderFolderItem (txt : string) (color : string) =
    Menu.Item.li [ ]
        [
            Icon.icon [ Icon.Props [ Style [ Color color ] ] ]
                [
                    Fa.i [ Fa.Solid.Folder ]
                        [ ]
                ]
            str txt
        ]

let private renderTagItem (txt : string) (color : string) =
    Menu.Item.li [ ]
        [
            Icon.icon [ Icon.Props [ Style [ Color color ] ] ]
                [
                    Fa.i [ Fa.Solid.Tag ]
                        [ ]
                ]
            str txt
        ]

let private sideMenu =
    Menu.menu [ CustomClass "sidebar-main" ]
        [
            Menu.list [ ]
                [ item "Inbox" Fa.Solid.Inbox true
                  item "Sent" Fa.Regular.Envelope false
                  item "Archive" Fa.Solid.Archive false
                  item "Stared" Fa.Solid.Star false
                  item "Trash" Fa.Regular.TrashAlt false
                ]

            Menu.label [ ]
                [
                    str "Folders"
                ]
            Menu.list [ ]
                [
                    renderFolderItem "Bills" "#e6984c"
                    renderFolderItem "OSS" "#c793ca"
                ]

            Menu.label [ ]
                [
                    str "Tags"
                ]
            Menu.list [ ]
                [
                    renderTagItem "Github" "#c793ca"
                    renderTagItem "Gitlab" "#c793ca"
                ]
        ]


let view (model : Model) (dispatch : Dispatch<Msg>) =
    let content =
        match model.Page with
        | Page.Inbox inboxModel ->
            Mailbox.Inbox.view inboxModel (InboxMsg >> dispatch)

    Columns.columns
        [ Columns.CustomClass "is-inbox"
          Columns.IsGapless ]
        [ Column.column
            [ Column.CustomClass "is-main-menu"
              Column.Width (Screen.All, Column.Is2) ]
            [ Text.div
                [ Modifiers
                    [ Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ]
                  Props
                    [ Style
                        [ Padding "2rem 2rem 1rem" ]
                    ]
                ]
                [ Button.button
                    [ Button.Color IsPrimary
                      Button.IsFullWidth
                      Button.Modifiers [ Modifier.TextWeight TextWeight.Bold ]
                    ]
                    [ str "Compose" ]
                ]
              sideMenu ]
          Column.column [ ]
            [ content ]
        ]
