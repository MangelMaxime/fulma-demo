module Router

open Fable.Import

type QuestionPage =
    | Index

type Page =
    | Question of QuestionPage

let toHash page =
    match page with
    | Question questionPage ->
        match questionPage with
        | Index -> "#question/index"

open Elmish.Browser.UrlParser

let defaultPage =
    Browser.console.error("Error parsing url: " + Browser.window.location.href)
    (QuestionPage.Index |> Question)

let pageParser: Parser<Page->Page,Page> =
    oneOf [
        map (QuestionPage.Index |> Question) (s "question" </> s "index")
        map defaultPage top ]
