module App.View

open Elmish
open Fable.React
open Fable.React.Props
open State
open Types
open Fable.FontAwesome
open Fable.FontAwesome.Free
open Fulma
open Fulma.Extensions.Wikiki

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

let item txt icon isActive =
    Menu.Item.li [ Menu.Item.IsActive isActive ]
        [ Icon.icon [ ]
            [ Fa.i [ icon ]
                [ ] ]
          str txt ]
let sideMenu =
    Menu.menu [ CustomClass "sidebar-main" ]
        [ Menu.list [ ]
            [ item "Inbox" Fa.Solid.Inbox true
              item "Sent" Fa.Regular.Envelope false
              item "Stared" Fa.Solid.Star false
              item "Trash" Fa.Regular.TrashAlt false ] ]

let email =
    Media.media [ Media.CustomClass "is-email-preview" ]
        [ Media.left [ ]
            [ Checkradio.checkbox [ Checkradio.Id "checkradio-1"
                                    Checkradio.Size IsSmall ]
                [ ] ]
          Media.content [ ]
            [ Heading.p [ Heading.Is6 ]
                [ str "Maxime Mangel" ]
              Heading.p [ Heading.Is6
                          Heading.IsSubtitle ]
                [ str "Lorem ipsum dolor sit amet, consectetur adipiscing" ] ]
          Media.right [ CustomClass "is-info-container" ]
            [ span [ Class "is-date" ]
                [ str "11 Jun 2019" ]
              Icon.icon [ ]
                [ Fa.i [ Fa.Regular.Star ]
                    [ ] ] ] ]

let emailsList =
    div [ ]
        [ Media.media [ Media.CustomClass "is-email" ]
            [ Media.left [ Props [ Style [ Border "1px solid black" ] ] ]
                [ Checkbox.checkbox [ ]
                    [ Checkbox.input [ ] ] ]
              Media.content [ Props [ Style [ Border "1px solid black" ] ] ]
                [ str "Fable - Newsletter #1" ]
              Media.right [ Props [ Style [ Border "1px solid black" ] ] ]
                [ str "11 Jun 2019" ] ] ]

let buttonIcon icon =
    Button.button [  ]
        [ Icon.icon [ ]
            [ Fa.i [ icon ]
                [ ] ] ]

let menubar =
    Level.level [ Level.Level.CustomClass "is-menubar"
                  Level.Level.Modifiers [ Modifier.IsMarginless ] ]
        [ Level.left [ ]
            [ Button.list [ Button.List.HasAddons ]
                [ buttonIcon Fa.Solid.LongArrowAltLeft ]
              Button.list [ Button.List.HasAddons ]
                [ buttonIcon Fa.Solid.Eye
                  buttonIcon Fa.Solid.EyeSlash ]
              Button.list [ Button.List.HasAddons ]
                [ buttonIcon Fa.Regular.TrashAlt
                  buttonIcon Fa.Solid.Archive
                  buttonIcon Fa.Solid.Ban ] ]
          Level.right [ ]
            [ Button.list [ Button.List.HasAddons ]
                [ buttonIcon Fa.Solid.AngleLeft
                  buttonIcon Fa.Solid.AngleDown
                  buttonIcon Fa.Solid.AngleRight ] ] ]

let pageContent =
    Columns.columns [ Columns.CustomClass "is-inbox"
                      Columns.IsGapless ]
        [ Column.column [ Column.CustomClass "is-main-menu"
                          Column.Width (Screen.All, Column.Is2) ]
            [ Text.div [ Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ]
                         Props [ Style [ Padding "2rem 2rem 1rem" ] ] ]
                [ Button.button [ Button.Color IsPrimary
                                  Button.IsFullWidth
                                  Button.Modifiers [ Modifier.TextWeight TextWeight.Bold ] ]
                    [ str "Compose" ] ]
              sideMenu ]
          Column.column [ ]
            [ menubar
              Columns.columns [ Columns.IsGapless ]
                [ Column.column [ Column.CustomClass "is-email-list"
                                  Column.Width (Screen.All, Column.Is5) ]
                    [ email
                      email
                      email
                      email
                      email
                      email
                      email
                      email
                      email
                      email
                      email
                      email
                      email
                      email
                      email
                      email
                      email
                      email
                      email
                      email ]
                  Column.column [ ]
                    [ ] ] ] ]

let private root model dispatch =
    let theme =
        if model.IsDark then
            "dark"
        else
            "light"

    div [ ]
        [ ofType<ThemeLoader.ThemeLoader,_,_> { Theme = theme } [ ]
          navbarView model.IsBurgerOpen dispatch
          Button.button [ Button.OnClick (fun _ ->
            dispatch ToggleTheme
          ) ]
            [ str "Toggle theme" ]
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
