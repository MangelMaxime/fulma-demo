module Question.Index.Types

type Model =
    { Questions : Question list option }

    static member Empty =
        { Questions = None }

type GetQuestionsRes =
    | Success of Question list
    | Error of exn

type Msg =
    | GetQuestions
    | GetQuestionsResult of GetQuestionsRes
