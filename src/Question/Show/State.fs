module Question.Show.State

open Elmish
open Types
open Okular
open Okular.Operators

let init id =
    Model.Empty id , Cmd.ofMsg (GetDetails id)

let verifyReply reply =
    if reply.Value <> "" then
        None
    else
        Some "Your answer can't be empty"

let update msg (model: Model) =
    match msg with
    | GetDetails id ->
        model, Cmd.ofPromise Rest.getDetails id (GetDetailsRes.Success >> GetDetailsResult) (GetDetailsRes.Error >> GetDetailsResult)

    | GetDetailsResult result ->
        match result with
        | GetDetailsRes.Success data ->
            { model with State = State.Success data }, Cmd.none

        | GetDetailsRes.Error error ->
            Logger.debugfn "[Question.Show.State] Error when fetching details: \n %A" error
            { model with State = State.Error }, Cmd.none
    | ChangeReply value ->
        model
        |> Lens.set (Model.ReplyLens >-> StringField.ValueLens) value
        |> Lens.set (Model.ReplyLens >-> StringField.ErrorLens) None, Cmd.none

    | Submit ->
        if model.IsWaitingReply then
            model, Cmd.none
        else
            match verifyReply model.Reply with
            | Some msg ->
                model
                |> Lens.set (Model.ReplyLens >-> StringField.ErrorLens) (Some msg), Cmd.none
            | None ->
                model, Cmd.none

            // if Validation.Show.verifyCreateAnswer createAnswerData then
            //     { model with IsWaitingReply = true }, Cmd.ofPromise Rest.createAnswer (model.Question.Value.Id, createAnswerData) CreateAnswerSuccess NetworkError
            // else
            //     model
            // |> Lens
