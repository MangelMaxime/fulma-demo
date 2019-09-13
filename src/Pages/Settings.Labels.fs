module Settings.Labels

open Fulma
open Elmish
open Fable.FontAwesome
open Fable.React
open Fable.React.Props
open System

type Model =
    {
        Labels : Map<Guid, obj>
    }

type Msg =
    | ChangeBody of string

let init () =
    {
        Labels = Map.empty
    }
    , Cmd.none

let update (msg  : Msg) (model : Model) =
    // match msg with
    // | ChangeBody newBody ->
    //     { model with
    //         Body = newBody
    //     }
    //     , Cmd.none
    model, Cmd.none


let view (model : Model) (dispatch : Dispatch<Msg>) =
    div [ ]
        [
            str "Settings.Labels"
        ]
