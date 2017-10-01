module Question.Dispatcher.State

open Elmish
open Types

let init (questionPage: Navigation.QuestionPage) =
    match questionPage with
    | Navigation.QuestionPage.Index ->
        let (subModel, subCmd) = Question.Index.State.init ()
        { Model.Empty with IndexModel = subModel }, Cmd.map IndexMsg subCmd

let update msg (model: Model) =
    match msg with
    | IndexMsg msg ->
        let (subModel, subCmd) = Question.Index.State.update msg model.IndexModel
        { model with IndexModel = subModel }, Cmd.map IndexMsg subCmd
