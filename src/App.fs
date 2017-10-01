module App.View

open Elmish
open Fable.Helpers.React
open Fable.Helpers.React.Props
open State

let root model dispatch =
    div [ ]
        [ str "Running" ]


open Elmish.React
open Elmish.Debug
open Elmish.HMR
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser

Program.mkProgram init update root
|> Program.toNavigable (parseHash Navigation.pageParser) urlUpdate
#if DEBUG
|> Program.withHMR
|> Program.withDebugger
#endif
|> Program.withReact "elmish-app"
|> Program.run
