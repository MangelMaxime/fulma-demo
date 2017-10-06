module Question.Show.Vote.State

open Elmish
open Types
open Okular
open Okular.Operators

let init user questionId answerId score =
    { QuestionId = questionId
      AnswerId = answerId
      Score = score
      Session = user }, Cmd.none

let update msg (model: Model) =
    match msg with
    | VoteUp ->
        model, Cmd.none

    | VoteDown ->
        model, Cmd.none
