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
    }

    member this.SortableKey =
        this.Email.Date

    member this.Key =
        this.Email.Guid

[<RequireQualifiedAccessAttribute>]
type ExternalMsg =
    | NoOp
    | Selected of Email

type Msg =
    | Select
    | Unselect

let init (email : Email) =
    {
        Email = email
        IsSelected = false
    }

let private notifyUnselect (guid : Guid) =
    let detail =
        jsOptions<Browser.Types.CustomEventInit>(fun o ->
            o.detail <- Some guid
        )

    let event = Browser.Event.CustomEvent.Create("inbox-email-meta-force-unselect", detail)

    window.dispatchEvent(event)
    |> ignore

let update (msg : Msg) (model : Model) =
    match msg with
    | Select ->
        if model.IsSelected then
            model
            , Cmd.none
            , ExternalMsg.NoOp

        else
            { model with
                IsSelected = true
            }
            , Cmd.OfFunc.execute notifyUnselect model.Email.Guid
            , ExternalMsg.Selected model.Email

    | Unselect ->
        { model with
            IsSelected = false
        }
        , Cmd.none
        , ExternalMsg.NoOp

type ViewProps =
    {
        Model : Model
        IsChecked : bool
        OnCheck : Guid -> unit
        Dispatch : Dispatch<Msg>
    }

let view =
    let formatDate =
        Date.Format.localFormat Date.Local.englishUK "dd MMM yyyy"

    FunctionComponent.Of(fun (props : ViewProps) ->

        Hooks.useEffectDisposable(fun () ->
            let listener =
                fun (ev : Browser.Types.Event) ->
                    let ev = ev :?>  Browser.Types.CustomEvent
                    let caller = ev.detail |> unbox<Guid>

                    if props.Model.IsSelected && props.Model.Key <> caller then
                        props.Dispatch Unselect

            window.addEventListener("inbox-email-meta-force-unselect", listener)

            { new System.IDisposable with
                member __.Dispose() =
                    window.removeEventListener("inbox-email-meta-force-unselect", listener)
            }
        , [| props.Model.IsSelected |])

        let mediaClass =
            Classes.fromListWithBase
                "is-email-preview"
                [
                    "is-active", props.Model.IsSelected
                ]

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
                                Checkradio.Checked props.IsChecked
                                // We need to attach an onClick listner instead of onChange becasue the parent is listening to onClick.
                                // If we use, onChange on the checkradio then both the child and parent events are triggering without having
                                // an easy way to prevent parent events.
                                Checkradio.LabelProps
                                    [
                                        OnClick (fun ev ->
                                            ev.stopPropagation()
                                            ev.preventDefault()
                                            props.OnCheck props.Model.Key
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
                        div [ ]
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

    , "Inbox.EmailMeta"
    , HMR.equalsButFunctions
    , (fun props -> props.Model.Key.ToString())
    )
