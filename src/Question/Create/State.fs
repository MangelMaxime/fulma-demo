module Question.Create.State

open Elmish
open Types
open System

let init _ : Model * Cmd<Msg> =
    Model.Empty, Cmd.none

let requiredField (value : string) =
    if String.IsNullOrWhiteSpace value then
        "This field is required"
    else
        ""

let validateModel model =
    let newModel =
        { model with TitleError = requiredField model.Title
                     ContentError = requiredField model.Content }

    let hasError =
        // Title is required
        String.IsNullOrWhiteSpace model.Title
        // Content is required
        || String.IsNullOrWhiteSpace model.Content

    newModel, hasError


let update (user : Database.User) msg (model: Model) =
    match msg with
    | ChangeTitle newTitle ->
        { model with Title = newTitle
                     TitleError = requiredField newTitle }, Cmd.none

    | ChangeContent newContent ->
        { model with Content = newContent
                     ContentError = requiredField newContent }, Cmd.none

    | Submit ->
        match validateModel model with
        | newModel, true ->
            newModel, Cmd.none

        | newModel, false ->
            { newModel with IsWaitingServer = true }, Cmd.OfPromise.either Rest.createQuestion
                                 (user.Id, model.Title, model.Content)
                                 (CreateQuestionRes.Success >> CreateQuestionResult)
                                 (CreateQuestionRes.Error >> CreateQuestionResult)

    | CreateQuestionResult (CreateQuestionRes.Success question) ->
        { model with IsWaitingServer = false }, Router.newUrl (Router.QuestionPage.Show question.Id |> Router.Question)

    | CreateQuestionResult (CreateQuestionRes.Error error) ->
        Logger.debugfn "[Question.Show.State] Error when fetching details: \n %A" error
        { model with IsWaitingServer = false }, Cmd.none
