module App.View

open Elmish
open Thoth.Elmish
open Fable.React
open Fable.React.Props
open Fable.FontAwesome
open Fable.FontAwesome.Free
open Fulma
open Fulma.Extensions.Wikiki
open Fable.Core
open Fable.Core.JsInterop

[<RequireQualifiedAccess>]
type Page =
    | Loading
    | Mailbox of Mailbox.Model
    | Settings of Settings.Model

type Model =
    {
        CurrentRoute : Router.Route
        ActivePage : Page
    }

type Msg =
    | MailboxMsg of Mailbox.Msg
    | SettingsMsg of Settings.Msg


let setRoute (result: Option<Router.Route>) (model : Model) =
    match result with
    | None ->
        let requestedUrl = Browser.Dom.window.location.href

        JS.console.error("Error parsing url: " + requestedUrl)

        model
        , Router.modifyUrl model.CurrentRoute

    | Some route ->
        match route with
        | Router.Mailbox mailboxRoute ->
            let (mailboxModel, mailboxCmd) = Mailbox.init mailboxRoute

            { model with
                ActivePage = Page.Mailbox mailboxModel
            }
            , Cmd.map MailboxMsg mailboxCmd

        | Router.Settings settingsRoute ->
            let (settingsModel, settingsCmd) = Settings.init settingsRoute

            { model with
                ActivePage = Page.Settings settingsModel
            }
            , Cmd.map SettingsMsg settingsCmd


let init (optRoute : Router.Route option) =
    let initialModel =
        {
            CurrentRoute =
                Router.MailboxRoute.Inbox None
                |> Router.Mailbox
            ActivePage = Page.Loading
        }

    setRoute optRoute initialModel


let private update (msg : Msg) (model : Model) =
    match msg with
    | MailboxMsg mailboxMsg ->
        match model.ActivePage with
        | Page.Mailbox mailboxModel ->
            let (mailboxModel, mailboxCmd) = Mailbox.update mailboxMsg mailboxModel

            { model with
                ActivePage = Page.Mailbox mailboxModel
            }
            , Cmd.map MailboxMsg mailboxCmd

        | _ ->
            model, Cmd.none

    | SettingsMsg settingsMsg ->
        match model.ActivePage with
        | Page.Settings settingsModel ->
            let (settingsModel, mailboxCmd) = Settings.update settingsMsg settingsModel

            { model with
                ActivePage = Page.Settings settingsModel
            }
            , Cmd.map SettingsMsg mailboxCmd

        | _ ->
            model, Cmd.none

let private root (model : Model) (dispatch : Dispatch<Msg>) =
    match model.ActivePage with
    | Page.Loading ->
        PageLoader.pageLoader [ PageLoader.IsActive true ]
            [ ]

    | Page.Mailbox mailboxModel ->
        div [ ]
            [ ofType<ThemeChanger.ThemeChanger,_,_> { Theme = "light" } [ ]
            //   navbarView model.IsBurgerOpen dispatch
            //   Button.button [ Button.OnClick (fun _ ->
            //     dispatch ToggleTheme
            //   ) ]
            //     [ str "Change theme" ]
              Navbar.view false ignore
              Mailbox.view mailboxModel (MailboxMsg >> dispatch) ]

    | Page.Settings settingsModel ->
        div [ ]
            [ ofType<ThemeChanger.ThemeChanger,_,_> { Theme = "light" } [ ]
            //   navbarView model.IsBurgerOpen dispatch
            //   Button.button [ Button.OnClick (fun _ ->
            //     dispatch ToggleTheme
            //   ) ]
            //     [ str "Change theme" ]
              Navbar.view false ignore
              Settings.view settingsModel (SettingsMsg >> dispatch) ]


let renderToastWithFulma =
    { new Toast.IRenderer<Fa.IconOption> with
        member __.Toast children color =
            Notification.notification [ Notification.CustomClass color ]
                children

        member __.CloseButton onClick =
            Notification.delete [ Props [ OnClick onClick ] ]
                [ ]

        member __.InputArea children =
            Columns.columns [ Columns.IsGapless
                              Columns.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ]
                              Columns.CustomClass "notify-inputs-area" ]
                children

        member __.Input (txt : string) (callback : (unit -> unit)) =
            Column.column [ ]
                [ Button.button [ Button.OnClick (fun _ -> callback ())
                                  Button.Color IsWhite ]
                    [ str txt ] ]

        member __.Title txt =
            Heading.h5 []
                       [ str txt ]

        member __.Icon (icon : Fa.IconOption) =
            Icon.icon [ Icon.Size IsMedium ]
                [ Fa.i [ icon
                         Fa.Size Fa.Fa2x ]
                    [ ] ]

        member __.SingleLayout title message =
            div [ ]
                [ title; message ]

        member __.Message txt =
            span [ ]
                 [ str txt ]

        member __.SplittedLayout iconView title message =
            Columns.columns [ Columns.IsGapless
                              Columns.IsVCentered ]
                [ Column.column [ Column.Width (Screen.All, Column.Is2) ]
                    [ iconView ]
                  Column.column [ ]
                    [ title
                      message ] ]

        member __.StatusToColor status =
            match status with
            | Toast.Success -> "is-success"
            | Toast.Warning -> "is-warning"
            | Toast.Error -> "is-danger"
            | Toast.Info -> "is-info" }

open Elmish.Debug
open Elmish.Navigation
open Elmish.UrlParser
open Elmish.HMR

// Init the first datas into the database
Database.Init()

Program.mkProgram init update root
|> Program.toNavigable (parseHash Router.pageParser) setRoute
|> Toast.Program.withToast renderToastWithFulma
|> Program.withReactSynchronous "elmish-app"
|> Program.run
