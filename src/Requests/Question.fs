module Requests.Question

open Fable.PowerPack
open Data.Forum
open Fable.Core.JsInterop

let getSummary _ =
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
                      Author =
                        { Id = user.Id
                          Firstname = user.Firstname
                          Surname = user.Surname
                          Avatar = user.Avatar }
                      Title = question.Title
                      Description = question.Description
                      CreatedAt = question.CreatedAt }
            )
            |> Array.toList

        do! Promise.sleep 500

        return result
    }

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
                        let anwsers =
                            question.Answers
                            |> Array.map (fun answer ->
                                Database.GetUserById answer.AuthorId
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
                            )

                        { Id = question.Id
                          Author = { Id = user.Id
                                     Firstname = user.Firstname
                                     Surname = user.Surname
                                     Avatar = user.Avatar }
                          Title = question.Title
                          Description = question.Description
                          CreatedAt = question.CreatedAt }, anwsers
            )

        do! Promise.sleep 500

        return data
    }
