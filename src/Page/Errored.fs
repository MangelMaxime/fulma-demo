namespace Page.Errored

open Views.Page

[<AutoOpen>]
module Types =

    type Model =
        { ActivePage : ActivePage
          ErrorMessage : string }

module State =

    let pageLoadError activePage errorMessage =
        { ActivePage = activePage
          ErrorMessage = errorMessage }

module View =

    open Fable.Helpers.React
    open Fable.Helpers.React.Props

    let root session model =
        div [ ClassName "" ]
            [ h1 [ ] [ str "Error loading page" ]
              p [ ]
                [ str model.ErrorMessage ]
            ]
