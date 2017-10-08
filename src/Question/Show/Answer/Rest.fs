module Question.Show.Answer.Rest

open Fable.PowerPack
open Types
open Database
open Fable.Core.JsInterop
open System

let voteUp (questionId, answerId) =
    promise {
        let answer =
            Database.Questions
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
            Database.Questions
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
