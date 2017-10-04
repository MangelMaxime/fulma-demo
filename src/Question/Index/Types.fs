module Question.Index.Types

open System

type QuestionInfo =
    { Id : int
      Author : Database.User
      Title : string
      Description : string
      CreatedAt : DateTime }

type Model =
    { Questions : QuestionInfo list option }

    static member Empty =
        { Questions = None }

type GetQuestionsRes =
    | Success of QuestionInfo list
    | Error of exn

type Msg =
    | GetQuestions
    | GetQuestionsResult of GetQuestionsRes
