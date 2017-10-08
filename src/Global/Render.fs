module Render

open Fulma.Layouts
open Fulma.Elements
open Fulma.BulmaClasses
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Import
open Fable.Core

let ``404 page`` =
    Hero.hero [ Hero.isFullHeight
                Hero.isDanger ]
        [ Hero.body [ ]
            [ Container.container [ Container.customClass Bulma.Properties.Alignment.HasTextCentered ]
                [ Heading.h1 [ ]
                    [ str "404" ] ] ] ]

let converter = Showdown.Globals.Converter.Create()

[<Pojo>]
type DangerousInnerHtml =
    { __html : string }

let contentFromMarkdown options str =
    Content.content
        [ yield! options
          yield Content.props [ DangerouslySetInnerHTML { __html =  converter.makeHtml str } ] ]
        [ ]
