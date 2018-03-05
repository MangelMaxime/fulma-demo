module Question.Create.Types

type Model =
    { Title : string
      Content : string
      TitleError : string
      ContentError : string
      IsWaitingServer : bool }

    static member Empty =
        { Title = ""
          Content = ""
          TitleError = ""
          ContentError = ""
          IsWaitingServer = false }

type CreateQuestionRes =
    | Success of Question
    | Error of exn

type Msg =
    | ChangeTitle of string
    | ChangeContent of string
    | Submit
    | CreateQuestionResult of CreateQuestionRes
