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
open Fulma.BulmaClasses

let navbarEnd =
    Navbar.end_div [ ]
        [ Navbar.item_div [ ]
            [ Field.field_div [ Field.isGrouped ]
                [ Control.control_p [ ]
                    [ Button.button [ Button.props [ Href "https://github.com/MangelMaxime/fulma-demo" ] ]
                        [ Icon.faIcon [ ] Fa.Github
                          span [ ] [ str "Source" ] ] ] ] ] ]

let navbarStart dispatch =
    Navbar.start_div [ ]
        [ Navbar.item_a [ Navbar.Item.props [ OnClick (fun _ ->
                                                        Router.QuestionPage.Index
                                                        |> Router.Question
                                                        |> Router.toHash
                                                        |> (fun url -> Browser.window.location.href <- url)) ] ]
            [ str "Home" ]
          Navbar.item_div [ Navbar.Item.hasDropdown
                            Navbar.Item.isHoverable ]
            [ Navbar.link_div [ ]
                [ str "Options" ]
              Navbar.dropdown_div [ ]
                [ Navbar.item_a [ Navbar.Item.props [ OnClick (fun _ -> dispatch ResetDatabase)] ]
                    [ str "Reset demo" ] ] ] ]

let navbarView isBurgerOpen dispatch =
    div [ ClassName "navbar-bg" ]
        [ Container.container [ ]
            [ Navbar.navbar [ Navbar.customClass "is-primary" ]
                [ Navbar.brand_div [ ]
                    [ Navbar.item_a [ Navbar.Item.props [ Href "#" ] ]
                        [ Image.image [ Image.is32x32 ]
                            [ img [ Src "assets/mini_logo.svg" ] ]
                          Heading.p [ Heading.is4 ]
                            [ str "Fulma-demo" ] ]
                      // Icon display only on mobile
                      Navbar.item_a [ Navbar.Item.props [ Href "https://github.com/MangelMaxime/fulma-demo" ]
                                      Navbar.Item.customClass "is-hidden-desktop" ]
                                    [ Icon.faIcon [ ] (unbox ("fa-lg " + string Fa.Github)) ] // TODO: Remove this hack to use the new icon API
                      // Make sure to have the navbar burger as the last child of the brand
                      Navbar.burger [ Fulma.Common.CustomClass (if isBurgerOpen then "is-active" else "")
                                      Fulma.Common.Props [
                                        OnClick (fun _ -> dispatch ToggleBurger) ] ]
                        [ span [ ] [ ]
                          span [ ] [ ]
                          span [ ] [ ] ] ]
                  Navbar.menu [ if isBurgerOpen then
                                    yield Navbar.Menu.isActive ]
                    [ navbarStart dispatch
                      navbarEnd ] ] ] ]

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
