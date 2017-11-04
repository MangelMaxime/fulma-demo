module Json.Parser

open Fable.PowerPack

let number a =
    match a with
    | Json.Number a -> Ok(a)
    | _ -> Error(System.Exception("Invalid JSON, it must be a number"))

let string a =
    match a with
    | Json.String a -> Ok(a)
    | _ -> Error(System.Exception("Invalid JSON, it must be a string"))

let lookup (key: string) (a: Map<string, Json.Json>) =
    match Map.tryFind key a with
    | Some(a) -> Ok(a)
    | None -> Error(System.Exception("Could not find key " + key))

let object a =
    match a with
    | Json.Object a -> Ok(Map.ofArray a)
    | _ -> Error(System.Exception("Invalid JSON, it must be an object"))

let array a =
    match a with
    | Json.Array a -> Ok(a)
    | _ -> Error(System.Exception("Invalid JSON, it must be an array"))

let toNumber key = lookup key >> (number |> Result.bind >> Result.unwrapResult)
let toString key = lookup key >> (string |> Result.bind >> Result.unwrapResult)
let toObject key = lookup key >> (object |> Result.bind >> Result.unwrapResult)
let toArray key = lookup key >> (array |> Result.bind >> Result.unwrapResult)
