module Navigation

type QuestionPage =
    | Index

type Page =
    | Home
    | Question of QuestionPage

let toHash page =
    match page with
    | Home -> "#home"
    | Question questionPage ->
        match questionPage with
        | Index -> "#question/index"

open Elmish.Browser.UrlParser

let pageParser: Parser<Page->Page,Page> =
    oneOf [
        map (QuestionPage.Index |> Question) (s "question" </> s "index")
        map Home top ]
