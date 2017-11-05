module Main

open Page

[<AutoOpen>]
module Types =

    type Author =
        { Id : int
          Firstname: string
          Surname: string
          Avatar : string }

    type Question =
        { Id : int
          Author : Author
          Title : string
          Description : string
          CreatedAt : string }

    type Page =
        | Blank
        | NotFound
        | Errored of Errored.Types.Model
        | Home of Home.Types.Model

    type PageState =
        | Loaded of Page
        | TransitioningFrom of Page

    type Model =
        { Session : User
          PageState : PageState }

    type Msg =
        | SetRoute of Router.Route option
        | HomeLoaded of Result<Home.Types.Model, Errored.Types.Model>
        | SomethingWentWrong of exn
        | HomeMsg of Home.Types.Msg

module State =

    open Elmish
    open Elmish.Browser.Navigation
    open Types
    open Fable.Import

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
            transition Home.State.init model.Session HomeLoaded

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
            { model with PageState = Loaded (Errored error) }, Cmd.none

        | (HomeMsg subMsg, Home subModel) ->
            toPage Home HomeMsg (Home.State.update session) subMsg subModel

        | (_, _) ->
            // Discard incoming messages that arrived for the wrong page
            model, Cmd.none

    let update msg model =
        updatePage (getPage model.PageState) msg model

    let init location =
        setRoute location
            { PageState = Loaded Blank
              Session = { Id = 10
                          Firstname = "Test"
                          Surname = "Test"
                          Avatar = "Test" } }

    let pageErrored model activePage errorMessage =
        let error = Errored.State.pageLoadError activePage errorMessage

        { model with PageState = Loaded (Errored error) }, Cmd.none

module View =

    open Elmish
    open Fable.Import
    open Fable.Helpers.React
    open Fable.Helpers.React.Props
    open State
    open Types
    open Fulma.Components
    open Fulma.Elements
    open Fulma.Elements.Form
    open Fulma.Extra.FontAwesome
    open Fulma.Layouts
    open Fulma.BulmaClasses
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
            Errored.View.root session subModel
            |> frame Page.Other

        | Home subModel ->
            Home.View.root session subModel (HomeMsg >> dispatch)
            |> frame Page.Other

    let root model dispatch =
        match model.PageState with
        | Loaded page ->
            viewPage model.Session false page dispatch
        | TransitioningFrom page ->
            viewPage model.Session true page dispatch

open Elmish
open Elmish.React
open Elmish.Debug
open Elmish.HMR
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser

// Init the first datas into the database
Database.Init()

Program.mkProgram State.init State.update View.root
|> Program.toNavigable (parseHash Router.pageParser) State.setRoute // To check and perhaps extend Navigation program to take a dispatcher instead of an update function
#if DEBUG
|> Program.withHMR
#endif
|> Program.withReact "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run
