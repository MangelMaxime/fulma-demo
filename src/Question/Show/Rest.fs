module Question.Show.Rest

open Fable.PowerPack
open Types
open Database

let getQuestions _ =
    promise {

        let questions =
            Database.Questions
                .sortBy("Id")
                .value()
            |> Array.toList

        do! Promise.sleep 1000

        return GetQuestionsRes.Success questions
    }
