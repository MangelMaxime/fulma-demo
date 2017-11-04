namespace Data.Question

open Data.Question.Author
open Fable.PowerPack.Json
open Fable.PowerPack.Result
open Json.Parser

type Question =
    { Id : int
      Author : Author
      Title : string
      Description : string
      CreatedAt : string }

    static member Decoder txt =
        result {
            let! json =
                txt
                |> ofString
                |> Result.bind object

            return
                { Id = json |> toNumber "Id" |> int
                  Author = json |> toObject "Author" |> Author.Decoder
                  Title = json |> toString "Title"
                  Description = json |> toString "Description"
                  CreatedAt = json |> toString "CreatedAt" }
        } |> unwrapResult
