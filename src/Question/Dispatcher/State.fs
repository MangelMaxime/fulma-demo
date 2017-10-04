module Question.Dispatcher.State

open Elmish
open Types

let init (questionPage: Router.QuestionPage) =
    // Store current page
    let model = { Model.Empty with CurrentPage = questionPage }
    // Store model depending on the current page
    match questionPage with
    | Router.QuestionPage.Index ->
        let (subModel, subCmd) = Question.Index.State.init ()
        { model with IndexModel = subModel }, Cmd.map IndexMsg subCmd

    | Router.QuestionPage.Show id ->
        let (subModel, subCmd) = Question.Show.State.init ()
        { model with ShowModel = subModel }, Cmd.map ShowMsg subCmd

let update msg (model: Model) =
    match msg with
    | IndexMsg msg ->
        let (subModel, subCmd) = Question.Index.State.update msg model.IndexModel
        { model with IndexModel = subModel }, Cmd.map IndexMsg subCmd

    | ShowMsg msg ->
        let (subModel, subCmd) = Question.Show.State.update msg model.ShowModel
        { model with ShowModel = subModel }, Cmd.map ShowMsg subCmd
