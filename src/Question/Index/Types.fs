module Question.Index.Types

type Model =
    { Questions : Question list option }

    static member Empty =
        { Questions = None }

type Msg =
    | GetQuestions
    | GetQuestionsSuccess of Question list
    | GetQuestionsError of exn
