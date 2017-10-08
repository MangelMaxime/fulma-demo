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
    { CurrentPage : Router.Page
      Session : User
      QuestionDispatcher : Question.Dispatcher.Types.Model option
      IsBurgerOpen : bool }

    static member Empty =
        { CurrentPage =
            Router.QuestionPage.Index
            |> Router.Question
          Session =
            let userId = 3
            match Database.GetUserById userId with
            | Some user -> user
            | None -> failwithf "User#%i not found" userId
          QuestionDispatcher = None
          IsBurgerOpen = false }

type Msg =
    | QuestionDispatcherMsg of Question.Dispatcher.Types.Msg
    | ResetDatabase
    | ToggleBurger
