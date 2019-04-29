module Render

open Fulma
open Fable.React
open Fable.React.Props

let pageNotFound =
    Hero.hero [ Hero.IsFullHeight
                Hero.Color IsDanger ]
        [ Hero.body [ ]
            [ Container.container [ Container.Modifiers [ Modifier.TextAlignment(Screen.All, TextAlignment.Centered) ] ]
                [ Heading.h1 [ ]
                    [ str "404" ] ] ] ]
