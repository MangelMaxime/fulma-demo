module Question.Dispatcher.Types

type Model =
    { CurrentPage : Router.QuestionPage
      IndexModel : Question.Index.Types.Model option
      ShowModel : Question.Show.Types.Model option
      Session : User }

    static member Empty user =
        { CurrentPage = Router.QuestionPage.Index
          IndexModel = None
          ShowModel = None
          Session = user }

type Msg =
    | IndexMsg of Question.Index.Types.Msg
    | ShowMsg of Question.Show.Types.Msg
