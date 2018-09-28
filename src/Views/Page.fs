module Views.Page

open Fable.Import
open Fable.Core.JsInterop
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma
open Fulma.Extensions
open Fulma.FontAwesome

type ActivePage =
    | Other
    | Home
    | Show
    | Option

let private toggleNavbarMenu (_ : Browser.Element) : unit = importMember "./../js/bulma-utils.js"

let private viewMenu currentPage =
    Navbar.menu [ Navbar.Menu.Props [ Id "navMenu" ] ]
        [ Navbar.Start.div [ ]
            [ Navbar.Item.a [ yield Navbar.Item.Props [ OnClick (fun _ -> Router.Home |> Router.modifyLocation ) ]
                              yield Navbar.Item.IsActive (currentPage = Home) ]
                [ str "Home" ]
              Navbar.Item.div [ yield Navbar.Item.HasDropdown
                                yield Navbar.Item.IsHoverable
                                yield Navbar.Item.IsActive (currentPage = Option) ]
                [ Navbar.Link.div [ ]
                    [ str "Options" ]
                  Navbar.Dropdown.div [ ]
                    [ Navbar.Item.a [ Navbar.Item.Props [ ] ]
                        [ str "Reset demo" ] ] ] ]
          Navbar.End.div [ ]
            [ Navbar.Item.div [ ]
                [ Field.div [ Field.IsGrouped ]
                    [ Control.p [ ]
                        [ Button.a [ Button.Props [ Href "https://github.com/MangelMaxime/fulma-demo" ] ]
                            [ Icon.faIcon [ ] [ Fa.icon Fa.I.Github ]
                              span [ ] [ str "Source" ] ] ] ] ] ] ]

let private viewHeader user currentPage =
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
                                    [ Icon.faIcon [ Icon.Size IsLarge ] [ Fa.icon Fa.I.Github ] ]
                      // Make sure to have the navbar burger as the last child of the brand
                      Navbar.burger [ Fulma.Common.GenericOption.Props [ Data("target","navMenu")
                                                                         Ref toggleNavbarMenu ] ]
                        [ span [ ] [ ]
                          span [ ] [ ]
                          span [ ] [ ] ] ]
                  viewMenu currentPage ] ] ]

let frame isLoading user page content =
    div [ ]
        [ PageLoader.pageLoader [ yield PageLoader.IsActive isLoading ]
            [ ]
          viewHeader user page
          content ]
