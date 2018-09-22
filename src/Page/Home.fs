namespace Page.Home

module Component =

    open Data.Forum

    open Elmish
    open Fable.Helpers.React
    open Fable.Helpers.React.Props
    open Fable.PowerPack
    open Fulma

    open Views

    type Model =
        { Questions : Question list }

    type Msg =
        | GetQuestions

    let init _ =
        Requests.Question.getSummary ()
        |> Promise.map (fun questions ->
            Ok { Questions = questions } )
        |> Promise.catch (fun error ->
            Error error.Message
        )

    let update session msg model =
        model, Cmd.none

    let view session (model : Model) dispatch =
        Container.container [ ]
            [ Section.section [ ]
                [ Heading.h3 [ ]
                    [ str "Latest questions" ] ]
              Columns.columns [ Columns.IsCentered ]
                [ Column.column [ Column.Width (Screen.All, Column.Is8) ]
                    (model.Questions |> List.map Question.viewSummary) ] ]
