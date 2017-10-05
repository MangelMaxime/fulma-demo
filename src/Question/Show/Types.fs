module Question.Show.Types

open System

type AnswerInfo =
    { CreatedAt : DateTime
      Author : Database.User
      Content : string }

type QuestionInfo =
    { Id : int
      Author : Database.User
      Title : string
      Description : string
      CreatedAt : DateTime
      Answers : AnswerInfo list }

type StringField =
    { Value : string
      Error : string option }

    static member Empty =
        { Value = ""
          Error = None }

type State =
    | Loading
    | Error
    | Success of QuestionInfo

type Model =
    { QuestionId : int
      State : State
      Reply : StringField
      IsWaitingReply : bool }

    static member Empty id =
        { QuestionId = id
          State = State.Loading
          Reply = StringField.Empty
          IsWaitingReply = false }

type GetDetailsRes =
    | Success of QuestionInfo
    | Error of exn

type Msg =
    | GetDetails of int
    | GetDetailsResult of GetDetailsRes
