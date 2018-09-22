module Render

open Fable.Core
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Import
open Fulma

let converter = Showdown.Globals.Converter.Create()

type DangerousInnerHtml =
    { __html : string }

let contentFromMarkdown options str =
    Content.content
        [ yield! options
          yield Content.Props [ DangerouslySetInnerHTML { __html =  converter.makeHtml str } ] ]
        [ ]
