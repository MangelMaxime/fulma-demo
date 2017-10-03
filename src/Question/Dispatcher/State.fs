module Question.Dispatcher.State

open Elmish
open Types

let init (questionPage: Navigation.QuestionPage) =
    // Store current page
    let model = { Model.Empty with CurrentPage = questionPage }
    // Store model depending on the current page
    match questionPage with
    | Navigation.QuestionPage.Index ->
        let (subModel, subCmd) = Question.Index.State.init ()
        { model with IndexModel = subModel }, Cmd.map IndexMsg subCmd

let update msg (model: Model) =
    match msg with
    | IndexMsg msg ->
        let (subModel, subCmd) = Question.Index.State.update msg model.IndexModel
        { model with IndexModel = subModel }, Cmd.map IndexMsg subCmd
