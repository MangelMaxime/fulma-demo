namespace Data.User

open Fable.PowerPack.Json
open Fable.PowerPack.Result
open Json.Parser

type User =
    { Id : int
      Firstname: string
      Surname: string
      Avatar : string }

    static member Decoder txt =
        result {
            let! json =
                txt
                |> ofString
                |> Result.bind object

            return
                { Id = json |> toNumber "Id" |> int
                  Firstname = json |> toString "Firstname"
                  Surname = json |> toString "Surname"
                  Avatar = json |> toString "Avatar" }
        } |> unwrapResult
