module Navbar

open Fulma
open Fable.FontAwesome
open Fable.React
open Fable.React.Props

let private navbarEnd =
    Navbar.End.div [ ]
        [ Navbar.Item.div [ ]
            [ Field.div [ Field.IsGrouped ]
                [ Control.p [ ]
                    [ Button.a
                        [ Button.Props
                            [ Href "https://github.com/MangelMaxime/fulma-demo" ]
                        ]
                        [ Icon.icon [ ]
                            [ Fa.i [ Fa.Brand.Github ]
                                [ ]
                            ]
                          span [ ]
                            [ str "Source" ]
                        ]
                    ]
                ]
            ]
        ]

let private navbarStart dispatch =
    Navbar.Start.div [ ]
        [ Navbar.Item.a
            [ Navbar.Item.Props
                [ OnClick (fun _ ->
                        Router.MailboxRoute.Inbox
                        |> Router.Mailbox
                        |> Router.modifyLocation
                    )
                ]
            ]
            [ str "Home"  ]

          Navbar.Item.div
            [ Navbar.Item.HasDropdown
              Navbar.Item.IsHoverable
            ]
            [ Navbar.Link.div [ ]
                [ str "Options" ]
              Navbar.Dropdown.div [ ]
                [ Navbar.Item.a
                    [ Navbar.Item.Props
                        [ //OnClick (fun _ -> dispatch ResetDatabase)
                        ]
                    ]
                    [ str "Reset demo" ]
                ]
            ]
        ]

let view isBurgerOpen dispatch =
    Navbar.navbar
        [ Navbar.Color IsPrimary
          Navbar.IsFixedTop ]
        [ Navbar.Brand.div [ ]
            [ Navbar.Item.a [ ]
                [ str "Fulma - Inbox" ]
            ]

          Navbar.End.div [ ]
            [ Navbar.Item.div
                [ Navbar.Item.HasDropdown
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
                        [ str "Logout" ]
                    ]
                ]
            ]
        ]
