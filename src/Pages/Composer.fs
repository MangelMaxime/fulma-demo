module Mailbox.Composer

open Fulma
open Elmish
open Fable.FontAwesome
open Fable.React
open Fable.React.Props
open System
open Helpers

type Model =
    {
        Composers : Map<Guid, Composer.Editor.Model>
    }

type Msg =
    | ComposerMsg of Guid * Composer.Editor.Msg

let init () =
    {
        Composers = Map.empty
    }

let update (msg  : Msg) (model : Model) =
    match msg with
    | ComposerMsg (guid, composerMsg) ->
        match Map.tryFind guid model.Composers with
        | Some composerModel ->
            let (composerModel, composerMsg) = Composer.Editor.update composerMsg composerModel
            { model with
                Composers = Map.add guid composerModel model.Composers
            }
            , Cmd.mapWithGuid ComposerMsg guid composerMsg

        | None ->
            model, Cmd.none

let private renderAction icon =
    Icon.icon [ Icon.Size IsMedium ]
        [
            Fa.i
                [
                    icon
                    Fa.Size Fa.FaLarge
                ]
                [ ]
        ]

let styleFromRank (rank : int) =
    Style
        [
            Transform (sprintf "translate(%ipx)" (rank * -600 + (rank + 1) * -20))
        ]

let view (model : Model) (dispatch : Dispatch<Msg>) =
    div [ Class "composer-container" ]
        [
            div
                [
                    Class "composer-editor"
                    styleFromRank 0
                ]
                [
                    div [ Class "composer-editor-header" ]
                        [
                            div [ Class "composer-editor-title" ]
                                [
                                    str "New message"
                                ]
                            div [ Class "composer-editor-actions" ]
                                [
                                    renderAction Fa.Solid.Compress
                                    renderAction Fa.Solid.Expand
                                    renderAction Fa.Solid.Times
                                ]
                        ]
                ]
            div
                [
                    Class "composer-editor is-expanded"
                    styleFromRank 1
                ]
                [
                    div [ Class "composer-editor-header" ]
                        [
                            div [ Class "composer-editor-title" ]
                                [
                                    str "New message"
                                ]
                            div [ Class "composer-editor-actions" ]
                                [
                                    renderAction Fa.Solid.Compress
                                    renderAction Fa.Solid.Expand
                                    renderAction Fa.Solid.Times
                                ]
                        ]
                    div [ Class "composer-editor-body" ]
                        [
                            Field.div
                                [
                                    Field.HasAddons
                                ]
                                [
                                    Control.div [ ]
                                        [
                                            Button.button [ Button.IsStatic true ]
                                                [
                                                    str "From"
                                                ]
                                        ]

                                    Control.div [ Control.IsExpanded ]
                                        [
                                            Input.input
                                                [
                                                    Input.CustomClass "is-fullwidth"
                                                    Input.IsReadOnly true
                                                    Input.Value "mangel.maxime@fulma.com"
                                                ]
                                        ]
                                ]

                            Field.div
                                [
                                    Field.HasAddons
                                ]
                                [
                                    Control.div [ ]
                                        [
                                            Button.button [ Button.IsStatic true ]
                                                [
                                                    str "To"
                                                ]
                                        ]

                                    Control.div [ Control.IsExpanded ]
                                        [
                                            Input.input
                                                [
                                                    Input.CustomClass "is-fullwidth"
                                                    Input.IsReadOnly true
                                                    Input.Value "mangel.maxime@fulma.com"
                                                ]
                                        ]
                                ]

                            Field.div [ ]
                                [
                                    Control.div [ Control.IsExpanded ]
                                        [
                                            Input.input
                                                [
                                                    Input.CustomClass "is-fullwidth"
                                                    // Input.IsReadOnly true
                                                    // Input.Value "mangel.maxime@fulma.com"
                                                    Input.Placeholder "Subject"
                                                ]
                                        ]
                                ]

                            Field.div [ ]
                                [
                                    Textarea.textarea
                                        [
                                            Textarea.CustomClass "composer-content-editor"
                                        ]
                                        [ ]
                                ]

                            Button.list
                                [
                                    Button.List.IsRight
                                    Button.List.Props
                                        [
                                            Style
                                                [
                                                    Padding ".5rem"
                                                ]
                                        ]
                                ]
                                [
                                    Button.button
                                        [
                                            Button.Color IsBlack
                                            Button.IsInverted
                                        ]
                                        [
                                            Icon.icon [ ]
                                                [
                                                    Fa.i
                                                        [
                                                            Fa.Solid.TrashAlt
                                                        ]
                                                        [ ]
                                                ]
                                        ]
                                    Button.button
                                        [
                                            Button.Color IsBlack
                                            Button.IsInverted
                                        ]
                                        [
                                            Icon.icon [ ]
                                                [
                                                    Fa.i
                                                        [
                                                            Fa.Solid.Save
                                                        ]
                                                        [ ]
                                                ]
                                        ]
                                    Button.button
                                        [
                                            Button.Color IsPrimary
                                        ]
                                        [
                                            str "SEND"
                                        ]
                                ]
                        ]
                ]
            div
                [
                    Class "composer-editor"
                    styleFromRank 2
                ]
                [
                    div [ Class "composer-editor-header" ]
                        [
                            div [ Class "composer-editor-title" ]
                                [
                                    str "New message"
                                ]
                            div [ Class "composer-editor-actions" ]
                                [
                                    renderAction Fa.Solid.Compress
                                    renderAction Fa.Solid.Expand
                                    renderAction Fa.Solid.Times
                                ]
                        ]
                ]
        ]
