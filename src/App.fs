module App.View

open Elmish
open Fable.Helpers.React
open Fable.Helpers.React.Props
open State
open Types
open Fulma
open Fulma.FontAwesome

let private navbarEnd =
    Navbar.End.div [ ]
        [ Navbar.Item.div [ ]
            [ Field.div [ Field.IsGrouped ]
                [ Control.p [ ]
                    [ Button.a [ Button.Props [ Href "https://github.com/MangelMaxime/fulma-demo" ] ]
                        [ Icon.faIcon [ ] [ Fa.icon Fa.I.Github ]
                          span [ ] [ str "Source" ] ] ] ] ] ]

let private navbarStart dispatch =
    Navbar.Start.div [ ]
        [ Navbar.Item.a [ Navbar.Item.Props [ OnClick (fun _ ->
                                                        Router.QuestionPage.Index
                                                        |> Router.Question
                                                        |> Router.modifyLocation) ] ]
            [ str "Home" ]
          Navbar.Item.div [ Navbar.Item.HasDropdown
                            Navbar.Item.IsHoverable ]
            [ Navbar.Link.div [ ]
                [ str "Options" ]
              Navbar.Dropdown.div [ ]
                [ Navbar.Item.a [ Navbar.Item.Props [ OnClick (fun _ -> dispatch ResetDatabase)] ]
                    [ str "Reset demo" ] ] ] ]

let private navbarView isBurgerOpen dispatch =
    div [ ClassName "navbar-bg" ]
        [ Container.container [ ]
            [ Navbar.navbar [ Navbar.CustomClass "is-primary" ]
                [ Navbar.Brand.div [ ]
                    [ Navbar.Item.a [ Navbar.Item.Props [ Href "#" ] ]
                        [ Image.image [ Image.Is32x32 ]
                            [ img [ Src "assets/mini_logo.svg" ] ]
                          Heading.p [ Heading.Is4 ]
                            [ str "Fulma-demo" ] ]
                      // Icon display only on mobile
                      Navbar.Item.a [ Navbar.Item.Props [ Href "https://github.com/MangelMaxime/fulma-demo" ]
                                      Navbar.Item.CustomClass "is-hidden-desktop" ]
                                    [ Icon.faIcon [ ] [ Fa.faLg; Fa.icon Fa.I.Github ] ]
                      // Make sure to have the navbar burger as the last child of the brand
                      Navbar.burger [ Fulma.Common.CustomClass (if isBurgerOpen then "is-active" else "")
                                      Fulma.Common.Props [
                                        OnClick (fun _ -> dispatch ToggleBurger) ] ]
                        [ span [ ] [ ]
                          span [ ] [ ]
                          span [ ] [ ] ] ]
                  Navbar.menu [ Navbar.Menu.IsActive isBurgerOpen ]
                    [ navbarStart dispatch
                      navbarEnd ] ] ] ]

let private renderPage model dispatch =
    match model with
    | { CurrentPage = Router.Question _
        QuestionDispatcher = Some extractedModel } ->
        Question.Dispatcher.View.root model.Session extractedModel (QuestionDispatcherMsg >> dispatch)
    | _ ->
        Render.pageNotFound

let private root model dispatch =
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
// Debugger is not working for Fable 2 (uses reflection and inflate/deflate)
// |> Program.withDebugger
#endif
|> Program.run
