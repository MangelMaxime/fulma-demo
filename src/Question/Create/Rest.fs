module Question.Create.Rest

open Database
open System

let createQuestion (userId : int, title : string, description : string) =
    promise {

        let nextId =
            Database.Questions
                .size()
                .value()
            |> unbox<int>
            |> (fun x -> x + 1)

        let question : Database.Question =
            { Id = nextId
              AuthorId = userId
              Title = title
              Description = description
              CreatedAt = DateTime.Now
              Answers = Array.empty }

        Database.Questions
            .push(question)
            .write()

        do! Promise.sleep 500

        return question
    }
