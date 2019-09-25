module Mailbox.Inbox.EmailMeta

open Fulma
open Fulma.Extensions.Wikiki
open Fable.React
open Fable.React.Props
open Fable.FontAwesome
open Elmish
open System
open Browser.Dom
open Helpers
open Fable.Core.JsInterop

type Model =
    {
        Email : Email
        IsSelected : bool
        IsChecked : bool
    }

    member this.SortableKey =
        this.Email.Date

    member this.Key =
        this.Email.Guid

[<RequireQualifiedAccessAttribute>]
type ExternalMsg =
    | NoOp
    | Selected of Email
    | UnSelect of Email
    | Checked of Email

[<RequireQualifiedAccessAttribute>]
type ReadResult =
    | Success of Email list
    | Errored of exn

type Msg =
    | Select
    | Unselect
    | Check
    | UpdateEmail of Email
    | ReadResult of ReadResult

let init (email : Email) =
    {
        Email = email
        IsSelected = false
        IsChecked = false
    }

let private notifyUnselect (guid : Guid) =
    let detail =
        jsOptions<Browser.Types.CustomEventInit>(fun o ->
            o.detail <- Some guid
        )

    let event = Browser.Event.CustomEvent.Create("inbox-email-meta-force-unselect", detail)

    window.dispatchEvent(event)
    |> ignore

let private notifyUnselectIfNotChecked (guid : Guid) =
    let detail =
        jsOptions<Browser.Types.CustomEventInit>(fun o ->
            o.detail <- Some guid
        )

    let event = Browser.Event.CustomEvent.Create("inbox-email-meta-force-unselect-if-not-checked", detail)

    window.dispatchEvent(event)
    |> ignore

let unselect (model : Model) =
    { model with
        IsSelected = false
        IsChecked = false
    }

let select (model : Model) =
    { model with
        IsSelected = true
        IsChecked = true
    }

type Context =
    {
        Session : Types.Session
    }

let private markAsRead (context : Context, guids : Guid list) =
    promise {
        let request = API.Email.markAsRead guids
        let! emails =
            request context.Session
            |> Promise.catchBind (API.Common.handleRefreshToken context.Session request)

        return emails
    }

let update (context : Context) (msg : Msg) (model : Model) =
    match msg with
    | Select ->
        if model.IsSelected then
            model
            , Cmd.none
            , ExternalMsg.NoOp

        else
            let cmds =
                Cmd.batch
                    [
                        Cmd.OfPromise.either
                            markAsRead
                            (context, [ model.Email.Guid ])
                            (ReadResult.Success >> ReadResult)
                            (ReadResult.Errored >> ReadResult)
                        Cmd.OfFunc.execute notifyUnselect model.Email.Guid
                    ]

            { model with
                IsSelected = true
            }
            , cmds
            , ExternalMsg.Selected model.Email

    | Unselect ->
        { model with
            IsSelected = false
            IsChecked = false
        }
        , Cmd.none
        , ExternalMsg.NoOp

    | Check ->
        if model.IsChecked then
            { model with
                IsSelected = false
                IsChecked = false
            }
            , Cmd.none
            , ExternalMsg.UnSelect model.Email

        else
            { model with
                IsChecked = true
                IsSelected = true
            }
            , Cmd.OfFunc.execute notifyUnselectIfNotChecked model.Email.Guid
            , ExternalMsg.Checked model.Email

    | UpdateEmail email ->
        { model with
            Email = email
        }
        , Cmd.none
        , ExternalMsg.NoOp

    | ReadResult result ->
        match result with
        | ReadResult.Success email ->
            model
            // Use the dedicated UpdateEmail message also used by the parent.
            // This helps mutualize the logic in a single plat
            , Cmd.ofMsg (UpdateEmail email.[0])
            , ExternalMsg.NoOp

        | ReadResult.Errored error ->
            Logger.errorfn "[Inbox.EmailMeta] An error occured when marking the email as read.\n%A" error

            model
            , Cmd.none
            , ExternalMsg.NoOp

type ViewProps =
    {
        Model : Model
        Dispatch : Dispatch<Msg>
    }

let view =
    let formatDate =
        Date.Format.localFormat Date.Local.englishUK "dd MMM yyyy"

    FunctionComponent.Of(fun (props : ViewProps) ->

        Hooks.useEffectDisposable(fun () ->
            let unselectListener =
                fun (ev : Browser.Types.Event) ->
                    let ev = ev :?>  Browser.Types.CustomEvent
                    let caller = ev.detail |> unbox<Guid>

                    if props.Model.IsSelected && props.Model.Key <> caller then
                        props.Dispatch Unselect

            let unselectListenerIFNotChecked =
                fun (ev : Browser.Types.Event) ->
                    let ev = ev :?>  Browser.Types.CustomEvent
                    let caller = ev.detail |> unbox<Guid>

                    if not props.Model.IsChecked && props.Model.Key <> caller then
                        props.Dispatch Unselect

            window.addEventListener("inbox-email-meta-force-unselect", unselectListener)
            window.addEventListener("inbox-email-meta-force-unselect-if-not-checked", unselectListenerIFNotChecked)

            { new System.IDisposable with
                member __.Dispose() =
                    window.removeEventListener("inbox-email-meta-force-unselect", unselectListener)
                    window.removeEventListener("inbox-email-meta-force-unselect-if-not-checked", unselectListenerIFNotChecked)
            }
        , [| props.Model.IsSelected; props.Model.IsChecked |])

        let mediaClass =
            Classes.fromListWithBase
                "is-email-preview"
                [
                    "is-active", props.Model.IsSelected
                    "is-read", props.Model.Email.IsRead
                    "is-unread", not props.Model.Email.IsRead
                ]

        div [ Class "email-preview-container" ]
            [
                Media.media
                    [
                        Media.CustomClass mediaClass
                        Media.Props
                            [
                                OnClick (fun _ ->
                                    props.Dispatch Select
                                )
                            ]
                    ]
                    [
                        Media.left [ ]
                            [
                                Checkradio.checkbox
                                    [
                                        Checkradio.Id (props.Model.Email.Guid.ToString())
                                        Checkradio.Color IsPrimary
                                        Checkradio.CustomClass "is-outlined"
                                        Checkradio.Checked props.Model.IsChecked
                                        // We need to attach an onClick listner instead of onChange becasue the parent is listening to onClick.
                                        // If we use, onChange on the checkradio then both the child and parent events are triggering without having
                                        // an easy way to prevent parent events.
                                        Checkradio.LabelProps
                                            [
                                                OnClick (fun ev ->
                                                    ev.stopPropagation()
                                                    ev.preventDefault()
                                                    props.Dispatch Check
                                                )
                                            ]
                                        // Set the input as read-only because we don't use onChange to detect the new state
                                        // This removes a react warning
                                        Checkradio.InputProps
                                            [
                                                ReadOnly true
                                            ]
                                    ]
                                    [ ]
                            ]
                        Media.content [ ]
                            [
                                div [ Class "email-subject" ]
                                    [ str props.Model.Email.Subject ]
                                div [ Class "email-sender" ]
                                    [ str props.Model.Email.From ]
                            ]
                        Media.right [ ]
                            [
                                props.Model.Email.Date
                                |> formatDate
                                |> str
                            ]
                    ]
            ]

    , "Inbox.EmailMeta"
    , HMR.equalsButFunctions
    , (fun props -> props.Model.Key.ToString())
    )
