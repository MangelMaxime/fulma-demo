module App.View

open Elmish
open Fable.Helpers.React
open Fable.Helpers.React.Props
open State
open Types

let renderPage model dispatch =
    match model.CurrentPage with
    | Navigation.Home ->
        str "Home"

    | Navigation.Question questionPage ->
        Question.Dispatcher.View.root model.QuestionDispatcher (QuestionDispatcherMsg >> dispatch)

let root model dispatch =
    renderPage model dispatch


open Elmish.React
open Elmish.Debug
open Elmish.HMR
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser

// Init the first datas into the database
Database.Init()

Program.mkProgram init update root
|> Program.toNavigable (parseHash Navigation.pageParser) urlUpdate
#if DEBUG
|> Program.withHMR
|> Program.withDebugger
#endif
|> Program.withReact "elmish-app"
|> Program.run
