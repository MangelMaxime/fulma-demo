module Router

open Browser
open Fable.React.Props
open Elmish.Navigation
open Elmish.UrlParser

type QuestionPage =
    | Index
    | Show of int
    | Create

type Page =
    | Question of QuestionPage
    | Home

let private toHash page =
    match page with
    | Question questionPage ->
        match questionPage with
        | Index -> "#question/index"
        | Show id -> sprintf "#question/%i" id
        | Create -> "#question/create"
    | Home -> "#/"

let pageParser: Parser<Page->Page,Page> =
    oneOf [
        map (QuestionPage.Index |> Question) (s "question" </> s "index")
        map (QuestionPage.Show >> Question) (s "question" </> i32)
        map (QuestionPage.Create |> Question) (s "question" </> s "create")
        map (QuestionPage.Index |> Question) top ]

let href route =
    Href (toHash route)

let modifyUrl route =
    route |> toHash |> Navigation.modifyUrl

let newUrl route =
    route |> toHash |> Navigation.newUrl

let modifyLocation route =
    window.location.href <- toHash route
