module Question.Show.Types

open System

type QuestionInfo =
    { Id : int
      Author : Database.User
      Title : string
      Description : string
      CreatedAt : DateTime }

type Model =
    { QuestionId : int
      Question : QuestionInfo option
      Answers : Answer.Types.Model list
      Reply : string
      Error : string
      IsWaitingReply : bool }

    static member Empty id =
        { QuestionId = id
          Question = None
          Answers = []
          Reply = ""
          Error = ""
          IsWaitingReply = false }

type GetDetailsRes =
    | Success of QuestionInfo * (Answer * User) []
    | Error of exn

type CreateAnswerRes =
    | Success of Answer * User
    | Error of exn

type Msg =
    | GetDetails of int
    | GetDetailsResult of GetDetailsRes
    | ChangeReply of string
    | Submit
    | CreateAnswerResult of CreateAnswerRes
    | AnswerMsg of int * Answer.Types.Msg
