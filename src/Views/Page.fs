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
        | Login

    let viewMenu =
        Navbar.menu [ ]
            [ Navbar.start_div [ ]
                [ Navbar.item_a [ Navbar.Item.props [ OnClick (fun _ ->
                                                                Router.QuestionPage.Index
                                                                |> Router.Question
                                                                |> Router.modifyLocation ) ] ]
                    [ str "Home" ]
                  Navbar.item_div [ Navbar.Item.hasDropdown
                                    Navbar.Item.isHoverable ]
                    [ Navbar.link_div [ ]
                        [ str "Options" ]
                      Navbar.dropdown_div [ ]
                        [ Navbar.item_a [ Navbar.Item.props [ ]]//OnClick (fun _ -> dispatch ResetDatabase)
                            [ str "Reset demo" ] ] ] ]
              Navbar.end_div [ ]
                [ Navbar.item_div [ ]
                    [ Field.field_div [ Field.isGrouped ]
                        [ Control.control_p [ ]
                            [ Button.button [ Button.props [ Href "https://github.com/MangelMaxime/fulma-demo" ] ]
                                [ Icon.faIcon [ ] Fa.Github
                                  span [ ] [ str "Source" ] ] ] ] ] ] ]

    let viewHeader user page =
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
                          Navbar.burger [ ]
                            [ span [ ] [ ]
                              span [ ] [ ]
                              span [ ] [ ] ] ]
                      viewMenu ] ] ]

    let frame isLoading user page content =
        div [ ]
            [ PageLoader.pageLoader [ if isLoading then
                                        yield PageLoader.isActive ]
                [ ]
              viewHeader user page
              content
            ]
