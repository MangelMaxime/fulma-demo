module Question.Show.Vote.Types

open System
open Okular.Lens

type Model =
    { QuestionId : int
      AnswerId : int
      Score : int
      Session : User }

type Msg =
    | VoteUp
    | VoteDown
