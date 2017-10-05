module Question.Show.State

open Elmish
open Types

let init id =
    Model.Empty id , Cmd.ofMsg (GetDetails id)

let update msg (model: Model) =
    match msg with
    | GetDetails id ->
        model, Cmd.ofPromise Rest.getDetails id GetDetailsResult (GetDetailsRes.Error >> GetDetailsResult)

    | GetDetailsResult result ->
        match result with
        | GetDetailsRes.Success data ->
            { model with Data = Some data }, Cmd.none

        | GetDetailsRes.Error error ->
            Logger.debugfn "[Question.Show.State] Error when fetching details: \n %A" error
            model, Cmd.none
