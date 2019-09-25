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
    | Login of Login.Model
    | AuthPage of AuthPage
    | NotFound

type Model =
    {
        CurrentRoute : Router.Route option
        ActivePage : Page
        Session : Types.Session option
    }

type Msg =
    | MailboxMsg of Mailbox.Msg
    | SettingsMsg of Settings.Msg
    | LoginMsg of Login.Msg
    | OnSessionChange of Types.Session
    | LogoutResult of LogoutResult

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
        | Router.Session sessionRoute ->
            match sessionRoute with
            | Router.SessionRoute.Restore ->
                match model.Session with
                | Some _ ->
                    model, Router.MailboxRoute.Inbox None
                            |> Router.Mailbox
                            |> Router.newUrl

                | None ->
                    model, Router.Login
                            |> Router.newUrl

            | Router.SessionRoute.Logout ->
                match model.Session with
                | Some session ->
                    let request (userId : Guid) =
                        promise {
                            do! API.User.logout userId
                            return LogoutResult.Success
                        }

                    model
                    , Cmd.OfPromise.either request session.UserId LogoutResult (LogoutResult.Errored >> LogoutResult)

                | None ->
                    model
                    , Router.Login
                        |> Router.newUrl

        | Router.Mailbox mailboxRoute ->
            match model.Session with
            | Some session ->
                let (mailboxModel, mailboxCmd) = Mailbox.init session mailboxRoute

                { model with
                    ActivePage =
                        AuthPage.Mailbox mailboxModel
                        |> Page.AuthPage
                }
                , Cmd.map MailboxMsg mailboxCmd

            | None ->
                model
                , Router.Login
                    |> Router.newUrl

        | Router.Settings settingsRoute ->
            let (settingsModel, settingsCmd) = Settings.init settingsRoute

            { model with
                ActivePage =
                    AuthPage.Settings settingsModel
                    |> Page.AuthPage
            }
            , Cmd.map SettingsMsg settingsCmd

        | Router.Login ->
            let loginModel = Login.init ()
            { model with
                ActivePage =
                    Page.Login loginModel
            }
            , Cmd.none


let private init (optRoute : Router.Route option) =
    match Session.tryGet () with
    | Some session ->
        {
            CurrentRoute = None
            ActivePage = Page.Loading
            Session = Some session
        }
        |> setRoute optRoute

    | None ->
        Router.modifyLocation Router.Route.Login

        {
            CurrentRoute = None
            ActivePage = Page.Loading
            Session = None
        }
        |> setRoute optRoute

let private update (msg : Msg) (model : Model) =
    match msg with
    | MailboxMsg mailboxMsg ->
        match model.ActivePage with
        | Page.AuthPage (AuthPage.Mailbox mailboxModel) ->
            match model.Session with
            | Some session ->
                let (mailboxModel, mailboxCmd) = Mailbox.update session mailboxMsg mailboxModel

                { model with
                    ActivePage =
                        AuthPage.Mailbox mailboxModel
                        |> Page.AuthPage
                }
                , Cmd.map MailboxMsg mailboxCmd

            | None ->
                model
                , Cmd.none

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

    | LoginMsg loginMsg ->
        match model.ActivePage with
        | Page.Login loginModel ->
            let (loginModel, loginCmd, extraMsg) = Login.update loginMsg loginModel

            let model =
                match extraMsg with
                | Login.ExternalMsg.NoOp ->
                    model

                | Login.ExternalMsg.SignIn session ->
                    Session.store session

                    { model with
                        Session = Some session
                    }

            { model with
                ActivePage =
                    Page.Login loginModel
            }
            , Cmd.map LoginMsg loginCmd

        | _ ->
            model, Cmd.none

    | OnSessionChange newSession ->
        Session.store newSession

        { model with
            Session = Some newSession
        }
        , Cmd.none

    | LogoutResult result ->
        match result with
        | LogoutResult.Success ->
            Session.delete()
            model
            , Router.Login
                |> Router.newUrl

        | LogoutResult.Errored error ->
            Logger.errorfn "[App] An error occured when try to logout.\n%A" error
            model
            , Cmd.none

type EventListenerProps =
    {
        Dispatch : Dispatch<Msg>
    }

type private EventListener(initProps) =
    inherit Component<EventListenerProps, obj>(initProps)

    let mutable closeEmailHandler = Unchecked.defaultof<Browser.Types.Event -> unit>

    override this.shouldComponentUpdate(nextProps, _) =
        HMR.equalsButFunctions this.props nextProps
        |> not

    override this.componentDidMount() =
        closeEmailHandler <-
            fun (ev : Browser.Types.Event) ->
                let ev = ev :?>  Browser.Types.CustomEvent
                let newSession = ev.detail |> unbox<Types.Session>

                this.props.Dispatch (OnSessionChange newSession)
                ()

        Browser.Dom.window.addEventListener("on-session-update", closeEmailHandler)

    override this.componentWillUnmount() =
        Browser.Dom.window.removeEventListener("on-session-update", closeEmailHandler)

    override this.render() =
        nothing

let private root (model : Model) (dispatch : Dispatch<Msg>) =
    match model.ActivePage with
    | Page.Loading ->
        PageLoader.pageLoader [ PageLoader.IsActive true ]
            [
                ofType<ThemeChanger.ThemeChanger,_,_> { Theme = "light" } [ ]
                ofType<EventListener,_,_> { Dispatch = dispatch } [ ]
            ]

    | Page.AuthPage authPage ->
        match authPage with
        | AuthPage.Mailbox mailboxModel ->
            div [ ]
                [
                    ofType<ThemeChanger.ThemeChanger,_,_> { Theme = "light" } [ ]
                    ofType<EventListener,_,_> { Dispatch = dispatch } [ ]
                    Navbar.view model.Session false ignore
                    Mailbox.view mailboxModel (MailboxMsg >> dispatch)
                ]

        | AuthPage.Settings settingsModel ->
            div [ ]
                [
                    ofType<ThemeChanger.ThemeChanger,_,_> { Theme = "light" } [ ]
                    ofType<EventListener,_,_> { Dispatch = dispatch } [ ]
                    Navbar.view model.Session false ignore
                    Settings.view settingsModel (SettingsMsg >> dispatch)
                ]

    | Page.Login loginModel ->
        div [ Style [ MarginTop "-3.1rem" ] ]
            [
                ofType<ThemeChanger.ThemeChanger,_,_> { Theme = "light" } [ ]
                ofType<EventListener,_,_> { Dispatch = dispatch } [ ]
                Login.view loginModel (LoginMsg >> dispatch)
            ]

    | Page.NotFound ->
        div [ Style [ MarginTop "-3.1rem" ] ]
            [
                ofType<ThemeChanger.ThemeChanger,_,_> { Theme = "light" } [ ]
                ofType<EventListener,_,_> { Dispatch = dispatch } [ ]
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
