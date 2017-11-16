namespace Page.Errored

module Component =

    open Views.Page

    type Model =
        { ActivePage : ActivePage
          ErrorMessage : string }

    let pageLoadError activePage errorMessage =
        { ActivePage = activePage
          ErrorMessage = errorMessage }

    open Fable.Helpers.React
    open Fable.Helpers.React.Props

    let view model =
        div [ ClassName "" ]
            [ h1 [ ] [ str "Error loading page" ]
              p [ ]
                [ str model.ErrorMessage ]
            ]
