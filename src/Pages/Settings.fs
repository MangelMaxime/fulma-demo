module Settings

open Fulma
open Elmish
open Fable.FontAwesome
open Fable.React
open Fable.React.Props
open System

[<RequireQualifiedAccess>]
type Page =
    | Labels of Settings.Labels.Model

type Model =
    {
        Page : Page
    }

type Msg =
    | LabelsMsg of Settings.Labels.Msg

let init (route : Router.SettingsRoute) =
    match route with
    | Router.SettingsRoute.Labels ->
        let (labelsModel, labelsCmd) = Settings.Labels.init ()
        {
            Page = Page.Labels labelsModel
        }
        , Cmd.map LabelsMsg  labelsCmd

    | _ ->
        failwithf "Route not supported yet: %A" route

let update (msg  : Msg) (model : Model) =
    model, Cmd.none


let view (model : Model) (dispatch : Dispatch<Msg>) =
    match model.Page with
    | Page.Labels labelsModel ->
        Settings.Labels.view labelsModel (LabelsMsg >> dispatch)
