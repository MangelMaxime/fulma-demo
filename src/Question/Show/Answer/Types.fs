module Question.Show.Answer.Types

open System

type Model =
    { QuestionId : int
      Answer : Answer
      Author : User
      Session : User
      IsLoading : bool
      Error : string }

type VoteUpRes =
    | Success of int
    | Error of exn

type Msg =
    | VoteUp
    | VoteDown
    | VoteUpResult of VoteUpRes
