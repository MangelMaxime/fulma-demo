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
open Helpers
open System


[<RequireQualifiedAccess>]
type AuthPage =
    | Mailbox of Mailbox.Model
    | Settings of Settings.Model

[<RequireQualifiedAccess>]
type LogoutResult =
    | Success
    | Errored of exn

[<RequireQualifiedAccess>]
type Page =
    | Loading
    | AuthPage of AuthPage
    | NotFound

type Model =
    {
        CurrentRoute : Router.Route option
        ActivePage : Page
    }

type Msg =
    | MailboxMsg of Mailbox.Msg
    | SettingsMsg of Settings.Msg

let private setRoute (result: Option<Router.Route>) (model : Model) =
    let model = { model with CurrentRoute = result }

    match result with
    | None ->
        let requestedUrl = Browser.Dom.window.location.href

        JS.console.error("Error parsing url: " + requestedUrl)

        { model with
            ActivePage = Page.NotFound
        }
        , Cmd.none

    | Some route ->
        match route with
        | Router.Mailbox mailboxRoute ->
            let (mailboxModel, mailboxCmd) = Mailbox.init mailboxRoute

            { model with
                ActivePage =
                    AuthPage.Mailbox mailboxModel
                    |> Page.AuthPage
            }
            , Cmd.map MailboxMsg mailboxCmd

        | Router.Settings settingsRoute ->
            let (settingsModel, settingsCmd) = Settings.init settingsRoute

            { model with
                ActivePage =
                    AuthPage.Settings settingsModel
                    |> Page.AuthPage
            }
            , Cmd.map SettingsMsg settingsCmd

let private init (optRoute : Router.Route option) =
    {
        CurrentRoute = None
        ActivePage = Page.Loading
    }
    |> setRoute optRoute

let private update (msg : Msg) (model : Model) =
    match msg with
    | MailboxMsg mailboxMsg ->
        match model.ActivePage with
        | Page.AuthPage (AuthPage.Mailbox mailboxModel) ->
            let (mailboxModel, mailboxCmd) = Mailbox.update mailboxMsg mailboxModel

            { model with
                ActivePage =
                    AuthPage.Mailbox mailboxModel
                    |> Page.AuthPage
            }
            , Cmd.map MailboxMsg mailboxCmd

        | _ ->
            model, Cmd.none

    | SettingsMsg settingsMsg ->
        match model.ActivePage with
        | Page.AuthPage (AuthPage.Settings settingsModel) ->
            let (settingsModel, mailboxCmd) = Settings.update settingsMsg settingsModel

            { model with
                ActivePage =
                    AuthPage.Settings settingsModel
                    |> Page.AuthPage
            }
            , Cmd.map SettingsMsg mailboxCmd

        | _ ->
            model, Cmd.none

let private root (model : Model) (dispatch : Dispatch<Msg>) =
    match model.ActivePage with
    | Page.Loading ->
        PageLoader.pageLoader [ PageLoader.IsActive true ]
            [
                ofType<ThemeChanger.ThemeChanger,_,_> { Theme = "light" } [ ]
            ]

    | Page.AuthPage authPage ->
        match authPage with
        | AuthPage.Mailbox mailboxModel ->
            div [ ]
                [
                    ofType<ThemeChanger.ThemeChanger,_,_> { Theme = "light" } [ ]
                    Navbar.view None false ignore
                    Mailbox.view mailboxModel (MailboxMsg >> dispatch)
                ]

        | AuthPage.Settings settingsModel ->
            div [ ]
                [
                    ofType<ThemeChanger.ThemeChanger,_,_> { Theme = "light" } [ ]
                    Navbar.view None false ignore
                    Settings.view settingsModel (SettingsMsg >> dispatch)
                ]

    | Page.NotFound ->
        div [ Style [ MarginTop "-3.1rem" ] ]
            [
                ofType<ThemeChanger.ThemeChanger,_,_> { Theme = "light" } [ ]
                Errored.notFound
            ]

open Elmish.Debug
open Elmish.Navigation
open Elmish.UrlParser
open Elmish.HMR

// Init the first datas into the database
Database.Init()

Program.mkProgram init update root
|> Program.toNavigable (parseHash Router.pageParser) setRoute
|> Toast.Program.withToast Toast.renderToastWithFulma
|> Program.withReactSynchronous "elmish-app"
|> Program.run
