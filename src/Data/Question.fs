namespace Data.Question

open Data.Question.Author
open Fable.PowerPack.Json
open Fable.PowerPack.Result
open Json.Parser
open System

type Question =
    { Id : int
      Author : Author
      Title : string
      Description : string
      CreatedAt : DateTime }

    static member Decoder (json: Map<string, Json>) =
        result {
            return
                { Id = json |> toNumber "Id" |> int
                  Author = json |> toObject "Author" |> Author.Decoder
                  Title = json |> toString "Title"
                  Description = json |> toString "Description"
                  CreatedAt = json |> toString "CreatedAt" |> DateTime.Parse }
        } |> unwrapResult

    static member Decoder (txt: string) =
        let json =
            txt
            |> ofString
            |> Result.bind object
            |> unwrapResult

        Question.Decoder(json)
