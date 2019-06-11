module App.View

open Elmish
open Fable.React
open Fable.React.Props
open State
open Types
open Fulma
open Fable.FontAwesome
open Fable.FontAwesome.Free

let private navbarEnd =
    Navbar.End.div [ ]
        [ Navbar.Item.div [ ]
            [ Field.div [ Field.IsGrouped ]
                [ Control.p [ ]
                    [ Button.a [ Button.Props [ Href "https://github.com/MangelMaxime/fulma-demo" ] ]
                        [ Icon.icon [ ]
                            [ Fa.i [ Fa.Brand.Github ] [ ] ]
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
    Navbar.navbar [ Navbar.Color IsPrimary
                    Navbar.IsFixedTop ]
        [ Navbar.Brand.div [ ]
            [ Navbar.Item.a [ ]
                [ //img [ Src "assets/logo_transparent.svg" ]
                  str "Fulma - Inbox" ] ]
          Navbar.End.div [ ]
            [ Navbar.Item.div [ Navbar.Item.HasDropdown
                                Navbar.Item.IsHoverable ]
                [ Navbar.Link.a [ ]
                    [  str "Account" ]
                  Navbar.Dropdown.div [ ]
                    [ Navbar.Item.a [ ]
                        [ str "Dashboard" ]
                      Navbar.Item.a [ ]
                        [ str "Profile" ]
                      Navbar.Item.a [ ]
                        [ str "Settings" ]
                      Navbar.divider [ ] [ ]
                      Navbar.Item.a [ ]
                        [ str "Logout" ] ] ] ] ]

let sideMenu =
    Menu.menu [ ]
        [ Menu.list [ ]
            [ Menu.Item.li [ ]
                [ Icon.icon [ ]
                    [ Fa.i [ Fa.Solid.Inbox ]
                        [ ] ]
                  str "Inbox" ]
              Menu.Item.li [ ] [ str "Sent" ]
              Menu.Item.li [ ] [ str "Stared" ]
              Menu.Item.li [ ] [ str "Trash" ] ] ]

let pageContent =
    Columns.columns [ Columns.CustomClass "is-inbox"
                      Columns.IsGapless ]
        [ Column.column [ Column.Width (Screen.All, Column.Is2) ]
            [ sideMenu ]
          Column.column [ ]
            [ ]
          Column.column [ ]
            [ ] ]

let private root model dispatch =
    div [ ]
        [ navbarView model.IsBurgerOpen dispatch
          pageContent ]

open Elmish.Debug
open Elmish.Navigation
open Elmish.UrlParser
open Elmish.HMR

// Init the first datas into the database
Database.Init()

Program.mkProgram init update root
|> Program.toNavigable (parseHash Router.pageParser) urlUpdate
#if DEBUG
|> Program.withConsoleTrace
#endif
|> Program.withReactSynchronous "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run
