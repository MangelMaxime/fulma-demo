module Mailbox

open Fulma
open Elmish
open Fable.FontAwesome
open Fable.React
open Fable.React.Props
open Types


type Settings =
    | Category of Email.Category


type Model =
    {
        ActiveCategory : Email.Category
        Inbox : Mailbox.Inbox.Model
        Composer : Mailbox.Composer.Model
        PageRank : int
    }


type Msg =
    | InboxMsg of Mailbox.Inbox.Msg
    | ComposerMsg of Mailbox.Composer.Msg


let private initInbox (session : Types.Session) (pageRank : int option) (category : Email.Category) =
    let pageRank = Option.defaultValue 1 pageRank

    let (inboxModel, inboxCmd) =
        Mailbox.Inbox.init
            {
                Session = session
                PageRank = pageRank
                Category = category
            }

    {
        ActiveCategory = category
        Inbox = inboxModel
        Composer = Mailbox.Composer.init ()
        PageRank = pageRank
    }
    , Cmd.map InboxMsg inboxCmd


let init (session : Types.Session) (route : Router.MailboxRoute) =
    match route with
    | Router.MailboxRoute.Inbox pageRank ->
        initInbox session pageRank Email.Category.Inbox

    | Router.MailboxRoute.Archive pageRank ->
        initInbox session pageRank Email.Category.Archive

    | Router.MailboxRoute.Sent pageRank ->
        initInbox session pageRank Email.Category.Sent

    | Router.MailboxRoute.Starred pageRank ->
        initInbox session pageRank Email.Category.Starred

    | Router.MailboxRoute.Trash pageRank ->
        initInbox session pageRank Email.Category.Trash

let update (session : Types.Session) (msg  : Msg) (model : Model) =
    match msg with
    | InboxMsg inboxMsg ->
        let (inboxModel, inboxCmd) =
            Mailbox.Inbox.update
                {
                    Session = session
                    PageRank = model.PageRank
                    Category = model.ActiveCategory
                }
                inboxMsg
                model.Inbox
        { model with
            Inbox = inboxModel
        }
        , Cmd.map InboxMsg inboxCmd

    | ComposerMsg composerMsg ->
        let (composerModel, composerCmd) = Mailbox.Composer.update composerMsg model.Composer
        { model with
            Composer = composerModel
        }
        , Cmd.map ComposerMsg composerCmd


let private standardCategoryItem
    (props :
        {|
            IsActive : bool
            Route : Router.Route
            Icon : Fa.IconOption
            Text : string
        |}
    ) =
    Menu.Item.li
        [
            Menu.Item.IsActive props.IsActive
            Menu.Item.OnClick (fun _ ->
                Router.modifyLocation props.Route
            )
        ]
        [
            Icon.icon [ ]
                [
                    Fa.i [ props.Icon ]
                        [ ]
                ]
            str props.Text
        ]

let private renderFolderItem (txt : string) (color : string) =
    Menu.Item.li [ Menu.Item.CustomClass "force-disabled-style" ]
        [
            Icon.icon [ Icon.Props [ Style [ Color color ] ] ]
                [
                    Fa.i [ Fa.Solid.Folder ]
                        [ ]
                ]
            str txt
        ]

let private renderTagItem (txt : string) (color : string) =
    Menu.Item.li [ Menu.Item.CustomClass "force-disabled-style" ]
        [
            Icon.icon [ Icon.Props [ Style [ Color color ] ] ]
                [
                    Fa.i [ Fa.Solid.Tag ]
                        [ ]
                ]
            str txt
        ]

let private sideMenu (model : Model) (dispatch : Dispatch<Msg>) =
    Menu.menu [ CustomClass "sidebar-main" ]
        [
            Menu.list [ ]
                [
                    standardCategoryItem
                        {|
                            IsActive =
                                model.ActiveCategory = Email.Category.Inbox
                            Route =
                                Router.MailboxRoute.Inbox None
                                |> Router.Mailbox
                            Icon = Fa.Solid.Inbox
                            Text = "Inbox"
                        |}
                    standardCategoryItem
                        {|
                            IsActive =
                                model.ActiveCategory = Email.Category.Sent
                            Route =
                                Router.MailboxRoute.Sent None
                                |> Router.Mailbox
                            Icon = Fa.Solid.Envelope
                            Text = "Sent"
                        |}
                    standardCategoryItem
                        {|
                            IsActive =
                                model.ActiveCategory = Email.Category.Archive
                            Route =
                                Router.MailboxRoute.Archive None
                                |> Router.Mailbox
                            Icon = Fa.Solid.Archive
                            Text = "Archive"
                        |}
                    standardCategoryItem
                        {|
                            IsActive =
                                model.ActiveCategory = Email.Category.Starred
                            Route =
                                Router.MailboxRoute.Starred None
                                |> Router.Mailbox
                            Icon = Fa.Solid.Star
                            Text = "Starred"
                        |}
                    standardCategoryItem
                        {|
                            IsActive =
                                model.ActiveCategory = Email.Category.Trash
                            Route =
                                Router.MailboxRoute.Trash None
                                |> Router.Mailbox
                            Icon = Fa.Solid.TrashAlt
                            Text = "Trash"
                        |}
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
    Columns.columns
        [
            Columns.CustomClass "is-inbox"
            Columns.IsGapless
        ]
        [
            Column.column
                [
                    Column.CustomClass "is-main-menu"
                    Column.Width (Screen.All, Column.Is2)
                ]
                [
                    Text.div
                        [
                            Modifiers
                                [ Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ]
                            Props
                                [ Style
                                    [ Padding "2rem 2rem 1rem" ]
                                ]
                        ]
                        [
                            Mailbox.Composer.composeButton (ComposerMsg >> dispatch)
                        ]
                    sideMenu model dispatch
                ]
            Column.column [ ]
                [
                    Mailbox.Composer.view model.Composer (ComposerMsg >> dispatch)
                    Mailbox.Inbox.view model.Inbox (InboxMsg >> dispatch)
                ]
        ]
