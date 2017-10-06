module Question.Show.Rest

open Fable.PowerPack
open Types
open Database
open Fable.Core.JsInterop
open System

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
                          CreatedAt = question.CreatedAt
                          Answers =
                            Array.map buildAnswersInfo question.Answers
                            |> Array.toList }
            )

        do! Promise.sleep 500

        return data
    }

let createAnswer (questionId : int, userId : int, content : string) =
    promise {

        let answer =
            { CreatedAt = DateTime.Now
              AuthorId = userId
              Content = content }

        // Add the answer to the question
        Database.Questions
            .find(createObj [ "Id" ==> questionId])
            .get(!^"Answers")
            .push(answer)
            .write()

        do! Promise.sleep 500

        return buildAnswersInfo answer
    }
