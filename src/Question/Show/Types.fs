module Question.Show.Types

open System
open Okular.Lens

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

    static member ValueLens =
        { Get = fun (r : StringField) -> r.Value
          Set = fun v (r : StringField) -> { r with Value = v } }

    static member ErrorLens =
        { Get = fun (r : StringField) -> r.Error
          Set = fun v (r : StringField) -> { r with Error = v } }

type Model =
    { QuestionId : int
      Data : QuestionInfo option
      Reply : StringField
      IsWaitingReply : bool
      Session : User }

    static member Empty user id =
        { QuestionId = id
          Data = None
          Reply = StringField.Empty
          IsWaitingReply = false
          Session = user }

    static member ReplyLens =
        { Get = fun (r : Model) -> r.Reply
          Set = fun v (r : Model) -> { r with Reply = v } }


type GetDetailsRes =
    | Success of QuestionInfo
    | Error of exn

type CreateAnswerRes =
    | Success of AnswerInfo
    | Error of exn

type Msg =
    | GetDetails of int
    | GetDetailsResult of GetDetailsRes
    | ChangeReply of string
    | Submit
    | CreateAnswerResult of CreateAnswerRes
