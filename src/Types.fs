module App.Types

type Author =
    { Id : int
      Firstname: string
      Surname: string
      Avatar : string }

type Question =
    { Id : int
      Author : Author
      Title : string
      Description : string
      CreatedAt : string }

type Model =
    { CurrentPage : Navigation.Page
      QuestionDispatcher : Question.Dispatcher.Types.Model }

    static member Empty =
        { CurrentPage = Navigation.Home
          QuestionDispatcher = Question.Dispatcher.Types.Model.Empty }

type Msg =
    | QuestionDispatcherMsg of Question.Dispatcher.Types.Msg
