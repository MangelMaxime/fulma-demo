module App.View

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

let navbarButton =
    Navbar.item_div [ ]
        [ Field.field_div [ Field.isGrouped ]
            [ Control.control_p [ ]
                [ Button.button [ Button.props [ Href "https://github.com/MangelMaxime/fulma-demo" ] ]
                    [ Icon.faIcon [ ] Fa.Github
                      span [ ] [ str "Source" ] ] ] ] ]

let navbarEnd =
    Navbar.end_div [ ]
        [ navbarButton ]

let navbarStart dispatch =
    Navbar.start_div [ ]
        [ Navbar.item_div [ Navbar.Item.hasDropdown
                            Navbar.Item.isHoverable ]
            [ Navbar.link_div [ ]
                [ str "Options" ]
              Navbar.dropdown_div [ ]
                [ Navbar.item_a [ Navbar.Item.props [ OnClick (fun _ -> dispatch ResetDatabase)] ]
                    [ str "Reset demo" ] ] ] ]

let navbarView dispatch =
    div [ ClassName "navbar-bg" ]
        [ Container.container [ ]
            [ Navbar.navbar [ Navbar.customClass "is-primary" ]
                [ Navbar.brand_a [ Fulma.Common.GenericOption.Props [ Href "#" ] ]
                    [ Navbar.item_div [ ]
                        [ Image.image [ Image.is32x32 ]
                            [ img [ Src "assets/mini_logo.svg" ] ]
                          Heading.p [ Heading.is4 ]
                            [ str "Fulma-demo" ] ] ]
                  navbarStart dispatch
                  navbarEnd ] ] ]

let renderPage model dispatch =
    match model with
    | { CurrentPage = Router.Question questionPage
        QuestionDispatcher = Some extractedModel } ->
        Question.Dispatcher.View.root extractedModel (QuestionDispatcherMsg >> dispatch)
    | _ ->
        Render.``404 page``

let root model dispatch =
    div [ ]
        [ navbarView dispatch
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
