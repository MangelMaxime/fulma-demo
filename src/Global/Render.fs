module Render

open Fulma.Layouts
open Fulma.Elements
open Fulma.BulmaClasses
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Import
open Fable.Core

let converter = Showdown.Globals.Converter.Create()

[<Pojo>]
type DangerousInnerHtml =
    { __html : string }

let contentFromMarkdown options str =
    Content.content
        [ yield! options
          yield Content.props [ DangerouslySetInnerHTML { __html =  converter.makeHtml str } ] ]
        [ ]
