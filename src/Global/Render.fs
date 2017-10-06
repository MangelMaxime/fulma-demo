module Render

open Fulma.Layouts
open Fulma.Elements
open Fulma.BulmaClasses
open Fable.Helpers.React

let ``404 page`` =
    Hero.hero [ Hero.isFullHeight
                Hero.isDanger ]
        [ Hero.body [ ]
            [ Container.container [ Container.customClass Bulma.Properties.Alignment.HasTextCentered ]
                [ Heading.h1 [ ]
                    [ str "404" ] ] ] ]
