module Question.Show.Answer.Types

type Model =
    { QuestionId : int
      Answer : Answer
      Author : User
      IsLoading : bool
      Error : string }

type VoteUpRes =
    | Success of int
    | Error of exn

type Msg =
    | VoteUp
    | VoteDown
    | VoteUpResult of VoteUpRes
