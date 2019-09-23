module Validation

open Thoth.Json
open Fulma
open System

type ErrorDef =
    { Text : string
      Key : string }

    static member Decoder =
        Decode.object
            (fun get ->
                { Text = get.Required.Field "text" Decode.string
                  Key = get.Required.Field "key" Decode.string } : ErrorDef )

let hasError key (errors : ErrorDef list) =
    errors
    |> List.exists (fun error ->
        error.Key = key
    )

let getError key (errors : ErrorDef list) =
    errors
    |> List.tryFind (fun error ->
        error.Key = key
    )
    |> function
        | Some error -> error.Text
        | None -> ""

let getErrorsByPartialKey (partialKey : string) (errors : ErrorDef list) =
    errors
    |> List.filter (fun error ->
        printfn "%A" error
        printfn "%A" (error.Key.Contains(partialKey))
        error.Key.Contains(partialKey)
    )
    |> List.map (fun error ->
        error.Text
    )

let removeError key (errors : ErrorDef list) =
    errors
    |> List.filter (fun error ->
        error.Key <> key
    )

let addError (key : string) (msg : string) (errors : ErrorDef list) =
    errors @ [ { Text = msg; Key = key } ]

let setError key text (errors : ErrorDef list) =
    if hasError key errors then
        errors
        |> List.map (fun error ->
            if error.Key = key then
                { error with Text = text }
            else
                error
        )
    else
        { Key = key
          Text = text } :: errors

let inputColor key errors =
    if hasError key errors then
        IsDanger
    else
        NoColor
    |> Input.Color

let textareaColor key errors =
    if hasError key errors then
        IsDanger
    else
        NoColor
    |> Textarea.Color

type Validator () =
    class end

    static member map2 (apply : 'T -> bool * 'T) (isOk : bool, model : 'T) =
        let (validationResult, newModel) = apply model

        validationResult && isOk, newModel

    static member map (apply : 'T -> bool * 'T) (model : 'T) =
        let (validationResult, newModel) = apply model

        validationResult, newModel


module Validate =

    open System
    open System.Text.RegularExpressions

    type StringValidator = string -> string -> ErrorDef list -> ErrorDef list

    let isOptional (value : string) (key : string) (validator : StringValidator) (errors : ErrorDef list) =
        if String.IsNullOrWhiteSpace value then
            errors
        else
            validator value key errors

    let isDateTimeString (value : string) (key : string) (errors : ErrorDef list) =
        let msg = "Ce champs doit être une date au format: YYYY-MM-DD hh:mm:ss"

        match Regex.Match(value.Trim(), "^\\d{4}-\\d{2}-\\d{2} \\d{2}:\\d{2}:\\d{2}$") with
        | m when m.Success -> errors
        | _ ->
            addError key msg errors
