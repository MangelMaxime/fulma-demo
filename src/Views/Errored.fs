module Errored

open Fable.React
open Fable.React.Props
open Fulma

let notFound =
    Hero.hero
        [
            Hero.IsFullHeight
        ]
        [
            Hero.body [ ]
                [
                    Columns.columns
                        [ Columns.Props [ Style [Width "100%"] ] ]
                        [
                            Column.column
                                [
                                    Column.Width(Screen.All, Column.IsOneThird)
                                    Column.Offset(Screen.All, Column.IsOneThird)
                                    Column.CustomClass "is-one-third is-offset-one-third"
                                ]
                                [
                                    Message.message [ Message.Color IsDanger ]
                                        [
                                            Message.body [ ]
                                                [ str "404 page not found" ]
                                        ]
                                ]
                        ]
                ]
        ]
