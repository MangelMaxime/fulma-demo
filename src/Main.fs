module Main

open Data.User

module Home = Page.Home.Component
module Errored = Page.Errored.Component
module Show = Page.Question.Show.Component

type Page =
    | Blank
    | NotFound
    | Errored of Errored.Model
    | Home of Home.Model
    | Show of Show.Model

type PageState =
    | Loaded of Page
    | TransitioningFrom of Page

type Model =
    { Session : User
      PageState : PageState }

type Msg =
    | SetRoute of Router.Route option
    | HomeLoaded of Result<Home.Model, string>
    | ShowLoaded of Result<Show.Model, string>
    | SomethingWentWrong of exn
    | HomeMsg of Home.Msg
    | ShowMsg of Show.Msg

open Elmish
open Elmish.Browser.Navigation

let getPage pageState =
    match pageState with
    | Loaded page ->
        page

    | TransitioningFrom page ->
        page

let setRoute (optRoute: Router.Route option) model =
    let transition task args onSuccess =
        { model with PageState = TransitioningFrom (getPage model.PageState) }, Cmd.ofPromise task args onSuccess SomethingWentWrong

    match optRoute with
    | None ->
        { model with PageState = Loaded NotFound }, Cmd.none

    | Some Router.Home ->
        transition Home.init () HomeLoaded

    | Some (Router.Question questionPage) ->
        match questionPage with
        | Router.QuestionPage.Show id ->
            transition Show.init id ShowLoaded
            // { model with PageState = Loaded NotFound }, Cmd.none

let updatePage page msg model =
    let session = model.Session
    let toPage toModel toMsg subUpdate subMsg subModel =
        let (newModel, newCmd) = subUpdate subMsg subModel
        { model with PageState = Loaded (toModel newModel) }, Cmd.map toMsg newCmd

    match (msg, page) with
    | SetRoute route, _ ->
        setRoute route model

    | (HomeLoaded (Ok subModel), _) ->
        { model with PageState = Loaded (Home subModel) }, Cmd.none

    | (HomeLoaded (Error error), _) ->
        { model with PageState = Loaded (Errored (Errored.pageLoadError Views.Page.Home error)) }, Cmd.none

    | (HomeMsg subMsg, Home subModel) ->
        toPage Home HomeMsg (Home.update session) subMsg subModel

    | (ShowLoaded (Ok subModel), _) ->
        { model with PageState = Loaded (Show subModel ) }, Cmd.none

    | (ShowLoaded (Error error), _) ->
        { model with PageState = Loaded (Errored (Errored.pageLoadError Views.Page.Show error )) }, Cmd.none

    | (ShowMsg subMsg, Show subModel) ->
        toPage Show ShowMsg (Show.update session) subMsg subModel

    | (subMsg, _) ->
        #if DEBUG
        printfn "Message discarded: %A" subMsg
        #endif
        // Discard incoming messages that arrived for the wrong page
        model, Cmd.none

let update msg model =
    updatePage (getPage model.PageState) msg model

let init location =
    setRoute location
        { PageState = Loaded Blank
          Session = { Id = 3
                      Firstname = "Guess"
                      Surname = ""
                      Avatar = "guess.png" } }

let pageErrored model activePage errorMessage =
    let error = Errored.pageLoadError activePage errorMessage

    { model with PageState = Loaded (Errored error) }, Cmd.none

open Fable.Helpers.React
open Views

let viewPage session isLoading page dispatch =
    let frame =
        Page.frame isLoading session

    match page with
    | NotFound ->
        str "page not found"

    | Blank ->
        str ""

    | Errored subModel ->
        Errored.view subModel
        |> frame Page.Other

    | Home subModel ->
        Home.view session subModel (HomeMsg >> dispatch)
        |> frame Page.Home

    | Show subModel ->
        Show.view session subModel (ShowMsg >> dispatch)
        |> frame Page.Show

let view model dispatch =
    match model.PageState with
    | Loaded page ->
        viewPage model.Session false page dispatch
    | TransitioningFrom page ->
        viewPage model.Session true page dispatch

open Elmish.React
open Elmish.Debug
open Elmish.HMR
open Elmish.Browser.UrlParser

// Init the first datas into the database
Database.Engine.Init()

Program.mkProgram init update view
|> Program.toNavigable (parseHash Router.pageParser) setRoute
#if DEBUG
|> Program.withHMR
#endif
|> Program.withReact "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run
