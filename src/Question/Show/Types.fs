module Question.Show.Types

type Data =
    { Questions : Question option
      Answers : Database.Answer list }

type StringField =
    { Value : string
      Error : string option }

    static member Empty =
        { Value = ""
          Error = None }

type Model =
    { QuestionId : int
      Data : Data option
      Reply : StringField
      IsWaitingReply : bool }

    static member Empty id =
        { QuestionId = id
          Data = None
          Reply = StringField.Empty
          IsWaitingReply = false }

type GetDetailsRes =
    | Success of Data
    | Error of exn

type Msg =
    | GetDetails of int
    | GetDetailsResult of GetDetailsRes
