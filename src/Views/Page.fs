namespace Views

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma.Components
open Fulma.Elements
open Fulma.Elements.Form
open Fulma.Extra.FontAwesome
open Fulma.Layouts
open Fulma.Extensions

module Page =

    type ActivePage =
        | Other
        | Home
        | Option

    let viewMenu currentPage =
        Navbar.menu [ ]
            [ Navbar.start_div [ ]
                [ Navbar.item_a [ yield Navbar.Item.props [ OnClick (fun _ ->
                                                                Router.Home
                                                                |> Router.modifyLocation ) ]
                                  if currentPage = Home then
                                        yield Navbar.Item.isActive ]
                    [ str "Home" ]
                  Navbar.item_div [ yield Navbar.Item.hasDropdown
                                    yield Navbar.Item.isHoverable
                                    if currentPage = Option then
                                        yield Navbar.Item.isActive ]
                    [ Navbar.link_div [ ]
                        [ str "Options" ]
                      Navbar.dropdown_div [ ]
                        [ Navbar.item_a [ Navbar.Item.props [ ] ]
                            [ str "Reset demo" ] ] ] ]
              Navbar.end_div [ ]
                [ Navbar.item_div [ ]
                    [ Field.field_div [ Field.isGrouped ]
                        [ Control.control_p [ ]
                            [ Button.button_a [ Button.props [ Href "https://github.com/MangelMaxime/fulma-demo" ] ]
                                [ Icon.faIcon [ ] [ Fa.icon Fa.I.Github ]
                                  span [ ] [ str "Source" ] ] ] ] ] ] ]

    let viewHeader user currentPage =
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
                                        [ Icon.faIcon [ Icon.isLarge ] [ Fa.icon Fa.I.Github ] ]
                          // Make sure to have the navbar burger as the last child of the brand
                          Navbar.burger [ ]
                            [ span [ ] [ ]
                              span [ ] [ ]
                              span [ ] [ ] ] ]
                      viewMenu currentPage ] ] ]

    let frame isLoading user page content =
        div [ ]
            [ PageLoader.pageLoader [ if isLoading then
                                        yield PageLoader.isActive ]
                [ ]
              viewHeader user page
              content ]
