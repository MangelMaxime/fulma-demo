module Requests.Answer

open Fable.PowerPack
open Data.Forum
open Fable.Core.JsInterop
open System

let createAnswer (questionId : int, userId : int, content : string) =
    promise {

        let nextId =
            Database.Engine.Questions
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
              Score = 0 } : Database.Answer

        // Add the answer to the question
        Database.Engine.Questions
            .find(createObj [ "Id" ==> questionId])
            .get(!^"Answers")
            .push(answer)
            .write()

        do! Promise.sleep 500

        return
            Database.Engine.GetUserById answer.AuthorId
            |> function
            | None -> failwith "Author of the answer unkown"
            | Some user ->
                { Id = answer.Id
                  CreatedAt = answer.CreatedAt
                  Author = { Id = user.Id
                             Firstname = user.Firstname
                             Surname = user.Surname
                             Avatar = user.Avatar }
                  Content = answer.Content
                  Score = answer.Score }
    }

let voteUp (questionId, answerId) =
    promise {
        let answer =
            Database.Engine.Questions
                .find(createObj [ "Id" ==> questionId ])
                .get(!^"Answers")
                .find(createObj [ "Id" ==> answerId ])

        let newScore =
            answer.value()
            |> unbox<Answer>
            |> (fun answer -> answer.Score + 1)

        answer
            .assign(createObj [ "Score" ==> newScore])
            .write()

        return newScore
    }

let voteDown (questionId, answerId) =
    promise {
        let answer =
            Database.Engine.Questions
                .find(createObj [ "Id" ==> questionId ])
                .get(!^"Answers")
                .find(createObj [ "Id" ==> answerId ])

        let newScore =
            answer.value()
            |> unbox<Answer>
            |> (fun answer -> answer.Score - 1)

        answer
            .assign(createObj [ "Score" ==> newScore])
            .write()

        return newScore
    }
