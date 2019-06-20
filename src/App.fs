module App.View

open Elmish
open Fable.React
open Fable.React.Props
open Fable.FontAwesome
open Fable.FontAwesome.Free
open Fulma
open Fulma.Extensions.Wikiki
open Fable.Core

[<RequireQualifiedAccess>]
type Page =
    | Mailbox of Mailbox.Model
    | Loading

type Model =
    {
        CurrentRoute : Router.Route
        ActivePage : Page
    }

type Msg =
    | MailboxMsg of Mailbox.Msg


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


let init (optRoute : Router.Route option) =
    let initialModel =
        {
            CurrentRoute =
                Router.MailboxRoute.Inbox
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

open Elmish.Debug
open Elmish.Navigation
open Elmish.UrlParser
open Elmish.HMR

// Init the first datas into the database
Database.Init()

Program.mkProgram init update root
|> Program.toNavigable (parseHash Router.pageParser) setRoute
|> Program.withReactSynchronous "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run
