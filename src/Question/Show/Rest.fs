module Question.Show.Rest

open Fable.PowerPack
open Types
open Database
open Fable.Core.JsInterop

let buildAnswersInfo (answer : Database.Answer) =
    Database.GetUserById answer.AuthorId
    |> function
       | None -> failwith "Author of the answer unkown"
       | Some user -> { CreatedAt = answer.CreatedAt
                        Author = user
                        Content = answer.Content }

let getDetails (id : int) =
    promise {

        let data =
            Database.Questions
                .find(createObj [ "Id" ==> id])
                .value()
            |> unbox<Database.Question>
            |> (fun question ->
                Database.GetUserById question.AuthorId
                |> function
                   | None -> failwith "Author of the question unkown"
                   | Some user ->
                        { Id = question.Id
                          Author = user
                          Title = question.Title
                          Description = question.Description
                          CreatedAt = question.CreatedAt }, Array.map buildAnswersInfo question.Answers
            )
            |> (fun (question, answers) ->
                { Question = question
                  Answers = Array.toList answers }
            )

        do! Promise.sleep 1000

        return data
    }
