module Mailbox.Composer.Editor

open Fulma
open Elmish
open Thoth.Elmish
open Fable.FontAwesome
open Fable.React
open Fable.React.Props
open System
open Helpers
open Browser
open Fable.Core.JsInterop

type Model =
    {
        To : string list
        ToValue : string option
        Body : string
        Subject : string
        Ancestor : Guid option
        CreatedAt : DateTime
        IsExpanded : bool
        IsWaitingServer : bool
        Errors : Validation.ErrorDef list
    }

    member this.SortableKey = this.CreatedAt

[<RequireQualifiedAccess>]
type SendResult =
    | Success
    | ValidationFailed of Validation.ErrorDef list
    | Errored of exn

type Msg =
    | ChangeBody of string
    | ChangeSubject of string
    | Collapse
    | Expand
    | RemoveTo of string
    | FocusTo
    | ChangeToValue of string
    | ValidateTo
    | CancelToInput
    | Send
    | SendResult of SendResult

let init (ancestor : Email option) =
    match ancestor with
    | Some ancestor ->
        {
            CreatedAt = DateTime.Now
            ToValue = None
            To = []
            Body =
                sprintf "\n\n\n\n\n-------%s" ancestor.Body
            Subject = "RE:" + ancestor.Subject
            Ancestor = Some ancestor.Guid
            IsExpanded = true
            IsWaitingServer = false
            Errors = []
        }

    | None ->
        {
            CreatedAt = DateTime.Now
            ToValue = None
            To = [ ]
            Body = ""
            Subject = ""
            Ancestor = None
            IsExpanded = true
            IsWaitingServer = false
            Errors = []
        }

let private askToClose (guid : Guid) =
    let detail =
        jsOptions<Types.CustomEventInit>(fun o ->
            o.detail <- Some guid
        )

    let event = CustomEvent.Create("composer-editor-ask-to-close", detail)

    window.dispatchEvent(event)
    |> ignore


let update (composerGuid : Guid) (msg  : Msg) (model : Model) =
    match msg with
    | ChangeBody newBody ->
        { model with
            Body = newBody
        }
        , Cmd.none

    | ChangeSubject newSubject ->
        { model with
            Subject = newSubject
        }
        , Cmd.none

    | Collapse ->
        { model with
            IsExpanded = false
        }
        , Cmd.none

    | Expand ->
        { model with
            IsExpanded = true
        }
        , Cmd.none

    | RemoveTo value ->
        { model with
            To =
                model.To
                |> List.filter (fun v ->
                    v <> value
                )
        }
        , Cmd.none

    | FocusTo ->
        match model.ToValue with
        | Some _ ->
            model
            , Cmd.none
        | None ->
            { model with
                ToValue = Some ""
            }
            , Cmd.none

    | ChangeToValue value ->
        { model with
            ToValue = Some value
        }
        , Cmd.none

    | ValidateTo ->
        match model.ToValue with
        | Some toValue ->
            { model with
                To =
                    if List.contains toValue model.To then
                        model.To
                    else
                        model.To @ [ toValue]
                ToValue = None
                Errors =
                    Validation.removeError "to" model.Errors
            }
            , Cmd.none
        | None ->
            model
            , Cmd.none

    | CancelToInput ->
        { model with
            ToValue = None

        }
        , Cmd.none

    | Send ->
        let sendEmailParameters =
            {
                From = "mangel.maxime@fulma.com"
                To = model.To |> List.toArray
                Subject = model.Subject
                Body = model.Body
                Tags = [||]
                Ancestor = model.Ancestor
            } : API.Email.SendEmailParameters

        let request (sendEmailParameters : API.Email.SendEmailParameters) =
            promise {
                let! res = API.Email.sendEmail sendEmailParameters

                match res with
                | Ok () ->
                    return SendResult.Success

                | Error errors  ->
                    return SendResult.ValidationFailed errors
            }

        { model with
            IsWaitingServer = true
        }
        , Cmd.OfPromise.either request sendEmailParameters SendResult (SendResult.Errored >> SendResult)

    | SendResult result ->
        match result with
        | SendResult.Success ->
            { model with
                IsWaitingServer = false
            }
            , Cmd.OfFunc.execute askToClose composerGuid

        | SendResult.ValidationFailed errors ->
            { model with
                IsWaitingServer = false
                Errors = errors
            }
            , Cmd.none

        | SendResult.Errored error ->
            Logger.errorfn "[Composer.Editor] An error occured.\n%A" error
            { model with
                IsWaitingServer = false
            }
            , Cmd.none


let private renderAction icon (triggerAction : unit -> unit) =
    Icon.icon
        [
            Icon.Size IsMedium
            Icon.Props
                [
                    OnClick (fun _ ->
                        triggerAction ()
                    )
                ]
        ]
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

let private renderToTag (dispatch : Dispatch<Msg>) (text : string) =
    Control.div
        [
            Control.Props
                [
                    Key ("to-tag---" + text)
                ]
        ]
        [
            Tag.list [ Tag.List.HasAddons ]
                [
                    Tag.tag
                        [
                            Tag.Props
                                [
                                    OnClick (fun ev ->
                                        ev.stopPropagation()
                                    )
                                ]
                        ]
                        [
                            str text
                        ]
                    Tag.delete
                        [
                            Tag.Props
                                [
                                    OnClick (fun ev ->
                                        dispatch (RemoveTo text)
                                        ev.stopPropagation()
                                    )
                                ]
                        ]
                        [ ]
                ]
        ]

let private renderToInput (toValue : string option) (dispatch : Dispatch<Msg>) =
    toValue
    |> Option.map (fun toValue ->
        Control.div [ ]
            [
                Input.input
                    [
                        Input.Value toValue
                        Input.OnChange (fun ev ->
                            ChangeToValue ev.Value
                            |> dispatch
                        )
                        Input.Props
                            [
                                AutoFocus true
                                OnKeyDown (fun ev ->
                                    if ev.keyCode = 13. then
                                        ev.preventDefault()
                                        dispatch ValidateTo
                                )
                                OnBlur (fun _ ->
                                    dispatch CancelToInput
                                )
                            ]
                    ]
            ]
    )
    |> Option.defaultValue nothing

let view (rank : int) (model : Model) (dispatch : Dispatch<Msg>) (onClose : unit -> unit) =
    let containerClass =
        Classes.fromListWithBase
            "composer-editor"
            [
                "is-expanded", model.IsExpanded
                "is-waiting-server", model.IsWaitingServer
            ]

    let titleText =
        if String.IsNullOrEmpty model.Subject then
            "New message"
        else
            model.Subject

    div
        [
            Class containerClass
            styleFromRank rank
        ]
        [
            div [ Class "composer-editor-container" ]
                [
                    div [ Class "composer-editor-header" ]
                        [
                            div [ Class "composer-editor-title" ]
                                [

                                    str titleText
                                ]
                            div [ Class "composer-editor-actions" ]
                                [
                                    renderAction Fa.Solid.Compress (fun () -> dispatch Collapse)
                                    renderAction Fa.Solid.Expand (fun () -> dispatch Expand)
                                    renderAction Fa.Solid.Times onClose
                                ]
                        ]
                    div [ Class "composer-editor-body" ]
                        [
                            div [ Class "waiting-server-overlay" ]
                                [ ]
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
                                    Field.CustomClass "composer-editor-body-to"
                                ]
                                [
                                    Control.div [ Control.CustomClass "composer-editor-body-to-label-control" ]
                                        [
                                            Button.button [ Button.IsStatic true ]
                                                [
                                                    str "To"
                                                ]
                                        ]

                                    Control.div
                                        [
                                            Control.IsExpanded
                                            Control.CustomClass "composer-editor-body-to-content-control"
                                            Control.Props
                                                [
                                                    OnClick (fun _ ->
                                                        dispatch FocusTo
                                                    )
                                                ]
                                        ]
                                        [
                                            Field.div
                                                [
                                                    Field.IsGrouped
                                                    Field.IsGroupedMultiline
                                                ]
                                                [
                                                    model.To
                                                    |> List.map (renderToTag dispatch)
                                                    |> ofList

                                                    renderToInput model.ToValue dispatch

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
                                                    Input.Value model.Subject
                                                    Input.OnChange (fun ev ->
                                                        ChangeSubject ev.Value
                                                        |> dispatch
                                                    )
                                                    Input.Placeholder "Subject"
                                                ]
                                        ]
                                ]

                            Field.div [ ]
                                [
                                    Textarea.textarea
                                        [
                                            Textarea.CustomClass "composer-content-editor"
                                            Textarea.Value model.Body
                                            Textarea.OnChange (fun ev ->
                                                ChangeBody ev.Value
                                                |> dispatch
                                            )
                                        ]
                                        [ ]
                                ]

                            Field.div
                                [
                                    Field.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ]
                                ]
                                [
                                    for error in model.Errors do
                                        yield Help.help [ Help.Color IsDanger ]
                                            [ str error.Text ]
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
                                            Button.IsLoading model.IsWaitingServer
                                            Button.OnClick (fun _ ->
                                                dispatch Send
                                            )
                                        ]
                                        [
                                            str "SEND"
                                        ]
                                ]
                        ]
                ]
        ]
