module Navigation

type Page =
    | Home

let toHash page =
    match page with
    | Home -> "#home"

open Elmish.Browser.UrlParser

let pageParser: Parser<Page->Page,Page> =
    oneOf [ map Home top ]
