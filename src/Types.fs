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
      Questions : Question list }

    static member Empty =
        { CurrentPage = Navigation.Home
          Questions =
            [ { Id = 0
                Author =
                    { Id = 0
                      Firstname = "Maxime"
                      Surname = "Mangel"
                      Avatar = "1.png" }
                Title = "What is the average wing speed of an unladen swallow?"
                Description =
                    """

                    """
                CreatedAt = "" }
              { Id = 0
                Author =
                    { Id = 0
                      Firstname = "Alfonso"
                      Surname = "Garciacaro"
                      Avatar = "1.png" }
                Title = "What is the average wing speed of an unladen swallow?"
                Description =
                    """

                    """
                CreatedAt = "" }
              { Id = 0
                Author =
                    { Id = 0
                      Firstname = "Robin"
                      Surname = "Munn"
                      Avatar = "1.png" }
                Title = "What is the average wing speed of an unladen swallow?"
                Description =
                    """

                    """
                CreatedAt = "" }
            ] }

type Msg =
    | Nothing
