module Question.Index.State

open Elmish
open Types

let init () =
    Model.Empty, Cmd.OfFunc.result GetQuestions

let update msg (model: Model) =
    match msg with
    | GetQuestions ->
        model, Cmd.OfPromise.either Rest.getQuestions () GetQuestionsResult (GetQuestionsRes.Error >> GetQuestionsResult)

    | GetQuestionsResult result ->
        match result with
        | GetQuestionsRes.Success questions ->
            { model with Questions = Some questions }, Cmd.none

        | GetQuestionsRes.Error error ->
            Logger.debugfn "[Question.Index.State] Error when fetch questions:\n %A" error
            model, Cmd.none
