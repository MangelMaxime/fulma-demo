module App.State

open Elmish
open Elmish.Browser.Navigation
open Types
open Fable.Import


let urlUpdate (result: Option<Navigation.Page>) model =
    match result with
    | None ->

        Browser.console.error("Error parsing url: " + Browser.window.location.href)
        model, Navigation.modifyUrl (Navigation.toHash model.CurrentPage)

    | Some page ->
        let model = { model with CurrentPage = page }
        match page with
        | Navigation.Home -> model, Cmd.none

let init result =
    urlUpdate result Model.Empty

let update msg model =
    model, Cmd.none

