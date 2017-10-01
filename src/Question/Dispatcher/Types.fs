module Question.Dispatcher.Types

type Model =
    { IndexModel : Question.Index.Types.Model }

    static member Empty =
        { IndexModel = Question.Index.Types.Model.Empty }

type Msg =
    | IndexMsg of Question.Index.Types.Msg
