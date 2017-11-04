namespace Data.Question.Author

open Fable.PowerPack.Json
open Fable.PowerPack.Result
open Json.Parser

type Author =
    { Id : int
      Firstname: string
      Surname: string
      Avatar : string }

    static member Decoder(json: Map<string,Json>) =
        result {
            return
                { Id = json |> toNumber "Id" |> int
                  Firstname = json |> toString "Firstname"
                  Surname = json |> toString "Surname"
                  Avatar = json |> toString "Avatar" }
        } |> unwrapResult

    static member Decoder(txt: string) =
        let json =
            txt
            |> ofString
            |> Result.bind object
            |> unwrapResult

        Author.Decoder(json)
