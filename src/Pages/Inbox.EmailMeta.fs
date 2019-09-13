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

[<RequireQualifiedAccess>]
type FetchEmailListResult =
    | Success of emails : Email list
    | Errored of exn

type Msg =
    | FetchEmailListResult of FetchEmailListResult
    | Select

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
            model, Cmd.none

        else
            { model with
                IsSelected = true
            }
            , Cmd.OfFunc.execute notifyUnselect model.Email.Guid

let private viewComponent =
    let formatDate =
        Date.Format.localFormat Date.Local.englishUK "dd MMM yyyy"

    FunctionComponent.Of(fun (props :
                                {|
                                    Model : Model
                                    Dispatch : Dispatch<Msg>
                                |}) ->

        Hooks.useEffectDisposable(fun () ->
            let listener =
                fun (ev : Browser.Types.Event) ->
                    let ev = ev :?>  Browser.Types.CustomEvent
                    printfn "Unselect all example: %A" ev.detail

            printfn "register effect"
            window.addEventListener("inbox-email-meta-force-unselect", listener)

            { new System.IDisposable with
                member __.Dispose() =
                    printfn "Disposing"
                    window.removeEventListener("inbox-email-meta-force-unselect", listener)
            }
        , [||])

        printfn "Maxime"

        Media.media
            [
                Media.CustomClass "is-email-preview"
                Media.Props
                    [
                        OnClick (fun _ ->
                            printfn "Clicked: %A" props.Model.Email.Guid
                            props.Dispatch Select
                        )
                    ]
            ]
            [ Media.left [ ]
                [
                    Checkradio.checkbox
                        [
                            Checkradio.Id (props.Model.Email.Guid.ToString())
                            Checkradio.Color IsPrimary
                            Checkradio.CustomClass "is-outlined"
                        ]
                        [ ]
                ]
              Media.content [ ]
                [ div [ ]
                    [ str props.Model.Email.Subject ]
                  Text.div
                    [
                         Modifiers
                            [
                                Modifier.TextWeight TextWeight.Bold
                                Modifier.TextColor IsGreyDark
                                Modifier.TextSize (Screen.All, TextSize.Is7)
                            ]
                     ]
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
    )

let view (model : Model) (dispatch : Dispatch<Msg>) =
    viewComponent
        {|
            Model = model
            Dispatch = dispatch
        |}
