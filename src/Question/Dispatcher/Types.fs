module Question.Dispatcher.Types

type Model =
    { CurrentPage : Router.QuestionPage
      IndexModel : Question.Index.Types.Model }

    static member Empty  =
        { CurrentPage = Router.QuestionPage.Index
          IndexModel = Question.Index.Types.Model.Empty }

type Msg =
    | IndexMsg of Question.Index.Types.Msg
