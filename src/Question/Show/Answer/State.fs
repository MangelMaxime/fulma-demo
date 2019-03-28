module Question.Show.Answer.State

open Elmish
open Types

let init questionId answer author =
    { QuestionId = questionId
      Answer = answer
      Author = author
      IsLoading = false
      Error = "" }, Cmd.none

let update msg (model: Model) =
    match msg with
    | VoteUp ->
        if model.IsLoading then
            model, Cmd.none
        elif model.Answer.Score = 5 then
            { model with Error = "You've already upvoted this answer 5 times, isn't that enough?" }, Cmd.none
        else
            { model with Error = ""
                         IsLoading = true }, Cmd.OfPromise.either
                                                Rest.voteUp
                                                (model.QuestionId, model.Answer.Id)
                                                (VoteUpRes.Success >> VoteUpResult)
                                                (VoteUpRes.Error >> VoteUpResult)

    | VoteDown ->
        if model.IsLoading then
            model, Cmd.none
        elif model.Answer.Score = -5 then
            { model with Error = "You've already downvoted this answer 5 times, isn't that enough?" }, Cmd.none
        else
            { model with Error = ""
                         IsLoading = true }, Cmd.OfPromise.either
                                                Rest.voteDown
                                                (model.QuestionId, model.Answer.Id)
                                                (VoteUpRes.Success >> VoteUpResult)
                                                (VoteUpRes.Error >> VoteUpResult)

    | VoteUpResult result ->
        match result with
        | Success newScore ->
            { model with IsLoading = false
                         Answer =
                            { model.Answer with Score = newScore } }, Cmd.none

        | Error error ->
            Logger.errorfn "[Question.Show.Answer.State] Error when upvoting the answer: \n%O" error
            model, Cmd.none
