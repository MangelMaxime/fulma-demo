module App.State

open Elmish
open Elmish.Browser.Navigation
open Types
open Fable.Import


let urlUpdate (result: Option<Router.Page>) model =
    match result with
    | None ->

        Browser.console.error("Error parsing url: " + Browser.window.location.href)
        model, Router.modifyUrl model.CurrentPage

    | Some page ->
        let model = { model with CurrentPage = page }
        match page with
        | Router.Question questionPage ->
            let (subModel, subCmd) = Question.Dispatcher.State.init model.Session questionPage
            { model with QuestionDispatcher = Some subModel }, Cmd.map QuestionDispatcherMsg subCmd

let init result =
    urlUpdate result Model.Empty

let update msg model =
    match (msg, model) with
    | (QuestionDispatcherMsg msg, { QuestionDispatcher = Some extractedModel }) ->
        let (subModel, subCmd) = Question.Dispatcher.State.update msg extractedModel
        { model with QuestionDispatcher = Some subModel }, Cmd.map QuestionDispatcherMsg subCmd

    | (QuestionDispatcherMsg capturedMsg, _) ->
        Browser.console.log("[App.State] Discarded message")
        printfn "%A" capturedMsg
        model, Cmd.none

    | (ResetDatabase, _) ->
        // Browser.localStorage.removeItem("database")
        Database.Restore()
        let redirect =
            Router.QuestionPage.Index
            |> Router.Question
            |> Router.newUrl

        model, redirect

    | (ToggleBurger, _) ->
        { model with IsBurgerOpen = not model.IsBurgerOpen }, Cmd.none
