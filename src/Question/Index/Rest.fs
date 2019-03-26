module Question.Index.Rest

open Types
open Database

let getQuestions _ =
    promise {

        let result =
            Database.Questions
                .sortBy("Id")
                .value()
            |> unbox<Database.Question []>
            |> Array.map(fun question ->
                match Database.GetUserById question.AuthorId with
                | None -> failwithf "Unkown author of id#%i for the question#%i" question.AuthorId question.Id
                | Some user ->
                    { Id = question.Id
                      Author = user
                      Title = question.Title
                      Description = question.Description
                      CreatedAt = question.CreatedAt }
            )
            |> Array.toList

        do! Promise.sleep 500

        return GetQuestionsRes.Success result
    }
