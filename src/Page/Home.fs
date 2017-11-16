namespace Page.Home

module Component =

    open Data.Forum

    type Model =
        { Questions : Question list }

    type Msg =
        | GetQuestions

    open Elmish
    open Views
    open Fable.PowerPack

    let init _ =
        Requests.Question.getSummary ()
        |> Promise.map (fun questions ->
            Ok { Questions = questions } )
        |> Promise.catch (fun error ->
            Error error.Message
        )

    let update session msg model =
        model, Cmd.none

    open Fable.Helpers.React
    open Fable.Helpers.React.Props
    open Fulma.Elements
    open Fulma.Layouts

    let view session (model : Model) dispatch =
        Container.container [ ]
            [ Section.section [ ]
                [ Heading.h3 [ ]
                    [ str "Latest questions" ] ]
              Columns.columns [ Columns.isCentered ]
                [ Column.column [ Column.Width.isTwoThirds]
                    (model.Questions |> List.map Question.viewSummary) ] ]
