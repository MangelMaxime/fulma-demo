module Question.Index.State

open Elmish
open Types

let init () =
    Model.Empty, Cmd.ofMsg GetQuestions

let update msg (model: Model) =
    match msg with
    | GetQuestions ->
        model, Cmd.none

    | GetQuestionsError error ->
        model, Cmd.none

    | GetQuestionsSuccess questions ->
        { model with Questions = Some questions }, Cmd.none
