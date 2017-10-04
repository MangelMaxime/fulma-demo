module Question.Index.Rest

open Fable.PowerPack
open Types
open Database
open Fable.Core.JsInterop

let associateUser (question : Database.Question) =
    Database.Users
        .find(createObj [ "Id" ==> question.AuthorId])
        .value()
    |> (fun (user : Database.User) ->
        { Id = question.Id
          Author = user
          Title = question.Title
          Description = question.Description
          CreatedAt = question.CreatedAt }
    )


let getQuestions _ =
    promise {

        let questions : Database.Question [] =
            Database.Questions
                .sortBy("Id")
                .value()

        let result =
            questions
            |> Array.toList
            |> List.map associateUser

        do! Promise.sleep 1000

        return GetQuestionsRes.Success result
    }
