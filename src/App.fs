module App.View

open Elmish
open Fable.Helpers.React
open Fable.Helpers.React.Props
open State
open Types
open Fulma.Components
open Fulma.Elements
open Fulma.Elements.Form
open Fulma.Extra.FontAwesome
open Fulma.Layouts

let navbarButton =
    Navbar.item_div [ ]
        [ Field.field_div [ Field.isGrouped ]
            [ Control.control_p [ ]
                [ Button.button [ ]
                    [ Icon.faIcon [ ] Fa.Github
                      span [ ] [ str "Fulma" ] ] ] ] ]

let navbarEnd =
    Navbar.end_div [ ]
        [ navbarButton ]

let navbarView =
    div [ ClassName "navbar-bg" ]
        [ Container.container [ ]
            [ Navbar.navbar [ ]
                [ Navbar.brand_a [ Fulma.Common.GenericOption.Props [ Href "#" ] ]
                    [ Navbar.item_div [ ]
                        [ Image.image [ Image.is32x32 ]
                            [ img [ Src "assets/mini_logo.svg" ] ]
                          Heading.p [ Heading.is4 ]
                            [ str "Fulma-demo" ] ] ]
                  navbarEnd ] ] ]

let renderPage model dispatch =
    match model.CurrentPage with
    | Router.Question questionPage ->
        Question.Dispatcher.View.root model.QuestionDispatcher (QuestionDispatcherMsg >> dispatch)

let root model dispatch =
    div [ ]
        [ navbarView
          renderPage model dispatch ]


open Elmish.React
open Elmish.Debug
open Elmish.HMR
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser

// Init the first datas into the database
Database.Init()

Program.mkProgram init update root
|> Program.toNavigable (parseHash Router.pageParser) urlUpdate
#if DEBUG
|> Program.withHMR
#endif
|> Program.withReact "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run
