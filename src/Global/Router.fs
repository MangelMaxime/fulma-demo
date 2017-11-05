[<RequireQualifiedAccess>]
module Router

open Fable.Import
open Fable.Helpers.React.Props
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser

type QuestionPage =
    | Show of int

type Route =
    | Question of QuestionPage
    | Home

let  toHash page =
    match page with
    | Home -> "#question/index"
    | Question questionPage ->
        match questionPage with
        | Show id -> sprintf "#question/%i" id

let pageParser: Parser<Route->Route,Route> =
    oneOf [
        map Home (s "question" </> s "index")
        map (QuestionPage.Show >> Question) (s "question" </> i32)
        map Home top ]

let inline href route =
    Href (toHash route)

let inline modifyUrl route =
    route |> toHash |> Navigation.modifyUrl

let inline newUrl route =
    route |> toHash |> Navigation.newUrl

let inline modifyLocation route =
    Browser.window.location.href <- toHash route
