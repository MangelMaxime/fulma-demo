namespace Main

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
        | HomeLoaded of Result<Errored.Types.Model, Home.Types.Model>

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

    let setRoute optRoute model =
        let transition toMsg cmd =
            { model with PageState = TransitioningFrom (getPage model.PageState) }, Cmd.map toMsg cmd

        match optRoute with
        | None ->
            { model with PageState = Loaded NotFound }, Cmd.none

        | Some Router.Home ->
            transition HomeLoaded (Home.State.init model.Session)

    let updatePage page msg model =
        match (msg, page) with
        | SetRoute route, _ ->
            setRoute route model

    let update msg model =
        updatePage (getPage model.PageState) msg model

    let init value location =
        // setRoute (Route.fromLocation location)
        //     { pageState = Loaded initialPage
        //     , session = { user = decodeUserFromJson value }
        //     }
        ()

    let pageErrored model activePage errorMessage =
        let error = Errored.State.pageLoadError activePage errorMessage

        { model with PageState = Loaded (Errored error) }, Cmd.none


    let setRoute optRoute model =
        let transition toMsg task =
            { model with PageState = TransitioningFrom (getPage model.PageState) }, Cmd.map toMsg task

        let errored = pageErrored model

        match optRoute with
        | None  ->
            { model with PageState = Loaded NotFound }, Cmd.none


        | Some Router.Index ->
            { model with PageState = Loaded (Index Index.initialModel) }, Cmd.none



    let update msg model =
        match (msg, model) with
        | (QuestionDispatcherMsg msg, { QuestionDispatcher = Some extractedModel }) ->
            let (subModel, subCmd) = Question.Dispatcher.State.update msg extractedModel
            { model with QuestionDispatcher = Some subModel }, Cmd.map QuestionDispatcherMsg subCmd

        | (QuestionDispatcherMsg capturedMsg, _) ->
            Browser.console.log("[App.State] Discarded message")
            printfn "%A" capturedMsg
            model, Cmd.none

        | (ResetDatabase, _) ->
            // Browser.localStorage.removeItem("database")
            Database.Restore()
            let redirect =
                Router.QuestionPage.Index
                |> Router.Question
                |> Router.toHash

            model, Navigation.newUrl redirect

        | (ToggleBurger, _) ->
            { model with IsBurgerOpen = not model.IsBurgerOpen }, Cmd.none

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

    let renderPage model dispatch =
        match model with
        | { CurrentPage = Router.Question questionPage
            QuestionDispatcher = Some extractedModel } ->
            Question.Dispatcher.View.root extractedModel (QuestionDispatcherMsg >> dispatch)
        | _ ->
            Render.``404 page``

    let root model dispatch =
        div [ ]
            [ navbarView model.IsBurgerOpen dispatch
              renderPage model dispatch ]

open Elmish
open Elmish.React
open Elmish.Debug
open Elmish.HMR
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser
open Page.Home

// Init the first datas into the database
Database.Init()

Program.mkProgram State.init State.update View.root
|> Program.toNavigable (parseHash Router.pageParser) State.urlUpdate
#if DEBUG
|> Program.withHMR
#endif
|> Program.withReact "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run
