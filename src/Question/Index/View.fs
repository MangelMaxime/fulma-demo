module Question.Index.View

open Types
open Fable.Helpers.React
open Fable.Helpers.React.Props

let root model dispatch =
    match model.Questions with
    | Some questions ->
        questions
        |> List.map (fun x ->
            str x.Author.Firstname
        )
        |> div []
    | None -> str "Loading"
