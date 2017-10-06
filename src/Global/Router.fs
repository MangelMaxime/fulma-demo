module Router

open Fable.Import

type QuestionPage =
    | Index
    | Show of int

type Page =
    | Question of QuestionPage

let toHash page =
    match page with
    | Question questionPage ->
        match questionPage with
        | Index -> "#question/index"
        | Show id -> sprintf "#question/%i" id

open Elmish.Browser.UrlParser

let pageParser: Parser<Page->Page,Page> =
    oneOf [
        map (QuestionPage.Index |> Question) (s "question" </> s "index")
        map (QuestionPage.Show >> Question) (s "question" </> i32)
        map (QuestionPage.Index |> Question) top ]
