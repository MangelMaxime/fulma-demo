module Mailbox.Composer.Editor

open Fulma
open Elmish
open Fable.FontAwesome
open Fable.React
open Fable.React.Props
open System

type Model =
    {
        To : string list
        Body : string
        Ancestor : Guid option
    }

type Msg =
    | ChangeBody of string

let init (ancestor : Email option) =
    match ancestor with
    | Some ancestor ->
        {
            To = []
            Body =
                sprintf "\n\n\n\n\n-------%s" ancestor.Body
            Ancestor = Some ancestor.Guid
        }

    | None ->
        {
            To = []
            Body = ""
            Ancestor = None
        }

let update (msg  : Msg) (model : Model) =
    match msg with
    | ChangeBody newBody ->
        { model with
            Body = newBody
        }
        , Cmd.none


let view (model : Model) (dispatch : Dispatch<Msg>) =
    div [ Class "composer-container" ]
        [
            str "email"
        ]
