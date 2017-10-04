module Question.Dispatcher.Types

type Model =
    { CurrentPage : Router.QuestionPage
      IndexModel : Question.Index.Types.Model
      ShowModel : Question.Show.Types.Model }

    static member Empty  =
        { CurrentPage = Router.QuestionPage.Index
          IndexModel = Question.Index.Types.Model.Empty
          ShowModel = Question.Show.Types.Model.Empty }

type Msg =
    | IndexMsg of Question.Index.Types.Msg
    | ShowMsg of Question.Show.Types.Msg
