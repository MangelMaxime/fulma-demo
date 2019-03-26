module Question.Show.Rest

open Types
open Database
open Fable.Core.JsInterop
open System

let assiocateAuthor (answer : Database.Answer) =
    Database.GetUserById answer.AuthorId
    |> function
       | None -> failwith "Author of the answer unkown"
       | Some user -> answer, user

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
                          CreatedAt = question.CreatedAt }, (Array.map assiocateAuthor question.Answers)
            )

        do! Promise.sleep 500

        return data
    }

let createAnswer (questionId : int, userId : int, content : string) =
    promise {

        let nextId =
            Database.Questions
                .find(createObj [ "Id" ==> questionId])
                .get(!^"Answers")
                .size()
                .value()
            |> unbox<int>
            |> (fun x -> x + 1)

        let answer =
            { Id = nextId
              CreatedAt = DateTime.Now
              AuthorId = userId
              Content = content
              Score = 0 }

        // Add the answer to the question
        Database.Questions
            .find(createObj [ "Id" ==> questionId])
            .get(!^"Answers")
            .push(answer)
            .write()

        do! Promise.sleep 500

        return assiocateAuthor answer
    }
