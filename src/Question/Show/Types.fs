module Question.Show.Types

open System
open Okular

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
        let get = fun (r : StringField) -> r.Value
        let set = fun v (r : StringField) -> { r with Value = v }
        Sugar.Lens get set

    static member ErrorOptional =
        let get = fun (r : StringField) -> r.Error
        let set = fun v (r : StringField) -> { r with Error = Some v }
        Sugar.Optional get set

type Data =
    { QuestionInfo : QuestionInfo
      VoteModel : Vote.Types.Model }

    static member QuestionInfoLens =
        let get = fun (r : Data) -> r.QuestionInfo
        let set = fun v (r : Data) -> { r with QuestionInfo = v }
        Sugar.Lens get set

    static member VoteModelLens =
        let get = fun (r : Data) -> r.VoteModel
        let set = fun v (r : Data) -> { r with VoteModel = v }
        Sugar.Lens get set

type Model =
    { QuestionId : int
      Data : Data option
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
        let get = fun (r : Model) -> r.Reply
        let set = fun v (r : Model) -> { r with Reply = v }
        Sugar.Lens get set

    static member DataOptional =
        let get = fun (r : Model) -> r.Data
        let set = fun v (r : Model) -> { r with Data = Some v }
        Sugar.Optional get set

let dataOfModel =
    let get = fun (r : Model) -> r.Data
    let set = fun v (r : Model) -> { r with Data = Some v }
    Sugar.Optional get set

let questionInfoOfdata =
    let get = fun (r : Data) -> r.QuestionInfo
    let set = fun v (r : Data) -> { r with QuestionInfo = v }
    Sugar.Lens get set

let questionInfoOfModel =
    Okular.Optional.composeLens dataOfModel questionInfoOfdata

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
