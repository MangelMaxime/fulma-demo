module Composer.Container

open Fulma
open Elmish
open Fable.FontAwesome
open Fable.React
open Fable.React.Props
open System

type Model =
    {
        Composers : Map<Guid, Composer.Model>
    }

type Msg =
    | ComposerMsg of Guid * Composer.Msg

let init () =
    {
        Composers = Map.empty
    }
    , Cmd.none

let update (msg  : Msg) (model : Model) =
    match msg with
    | ComposerMsg (guid, composerMsg) ->
        match Map.tryFind guid model.Composers with
        | Some composerModel ->
            let (composerModel, composerMsg) = Composer.update composerMsg composerModel
            { model with
                Composers = Map.add guid composerModel model.Composers
            }
            , Cmd.mapWithGuid ComposerMsg guid composerMsg

        | None ->
            model, Cmd.none


let view (model : Model) (dispatch : Dispatch<Msg>) =
    div [ Class "composer" ]
        [
            str "email"
        ]
