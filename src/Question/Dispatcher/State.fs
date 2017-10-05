module Question.Dispatcher.State

open Elmish
open Types
open Fable.Import

let init (questionPage: Router.QuestionPage) =
    // Store current page
    let model = { Model.Empty with CurrentPage = questionPage }
    // Store model depending on the current page
    match questionPage with
    | Router.QuestionPage.Index ->
        let (subModel, subCmd) = Question.Index.State.init ()
        { model with IndexModel = Some subModel }, Cmd.map IndexMsg subCmd

    | Router.QuestionPage.Show id ->
        let (subModel, subCmd) = Question.Show.State.init id
        { model with ShowModel = Some subModel }, Cmd.map ShowMsg subCmd

let update msg (model: Model) =
    match msg, model with
    | IndexMsg msg, { IndexModel = Some extractedModel } ->
        let (subModel, subCmd) = Question.Index.State.update msg extractedModel
        { model with IndexModel = Some subModel }, Cmd.map IndexMsg subCmd

    | ShowMsg msg, { ShowModel = Some extractedModel } ->
        let (subModel, subCmd) = Question.Show.State.update msg extractedModel
        { model with ShowModel = Some subModel }, Cmd.map ShowMsg subCmd
    | _ ->
        Browser.console.log("[Question.Dispatcher.State] Discarded message")
        model, Cmd.none
