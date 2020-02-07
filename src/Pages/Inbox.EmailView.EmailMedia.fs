module Mailbox.Inbox.EmailView.EmailMedia

open Fulma
open Fulma.Extensions.Wikiki
open Fable.React
open Fable.React.Props
open Fable.FontAwesome
open Elmish
open Fable.Core.JsInterop

type Model =
    {
        Email : Email
        IsExpanded : bool
    }

[<RequireQualifiedAccess>]
type FetchEmailHistoryResult =
    | Success of emails : Email list
    | Errored of exn

type Msg =
    | ToggleState

let init (email : Email) =
    {
        IsExpanded = false
        Email = email
    }
    , Cmd.none

let update (msg : Msg) (model : Model) =
    match msg with
    | ToggleState ->
        { model with
            IsExpanded = not model.IsExpanded
        }
        , Cmd.none

let formatDate =
    Date.Format.localFormat Date.Local.englishUK "ddd dd/MM/yyyy hh:mm"

let converter = Showdown.Globals.Converter.Create()


let buttonIcon icon (action : unit -> unit) =
    Button.button 
        [ 
            Button.OnClick (fun ev ->
                ev.stopPropagation()
                ev.preventDefault()
                action ()
            )
        ]
        [ Icon.icon [ ]
            [ Fa.i [ icon ]
                [ ]
            ]
        ]

let dropdownAction icon =
    Dropdown.dropdown
        [
            Dropdown.Props
                [
                    OnClick (fun ev ->
                        ev.stopPropagation()
                    )
                ]
        ]
        [
            div [ ]
                [ Button.button [ ]
                    [ Icon.icon [ ]
                        [ Fa.i [ icon ]
                            [ ] ]
                      Icon.icon [ Icon.Size IsSmall ]
                        [ Fa.i [ Fa.Solid.AngleDown ]
                            [ ] ] ] ]
            Dropdown.menu [ ]
                [ ]
        ]

let private emailMediaContent (model : Model) (dispatch : Dispatch<Msg>) =
    let recipients =
        model.Email.To
        |> String.concat ","

    div [ Class "email-media-content" ]
        [
            div [ Class "email-media-content-header" ]
                [
                    div [ Class "email-media-content-header-summary" ]
                        [
                            div [ Class "email-media-content-header-summary-left" ]
                                [
                                    div [ Class "email-media-sender" ]
                                        [ str model.Email.From ]

                                    div [ Class "email-media-date" ]
                                        [ str (formatDate model.Email.Date) ]
                                ]

                            div [ Class "email-media-content-header-summary-right" ]
                                [
                                    dropdownAction Fa.Solid.Folder
                                    dropdownAction Fa.Solid.Tag
                                    Button.list [ Button.List.HasAddons ]
                                        [
                                            buttonIcon Fa.Solid.Reply (fun _ -> 
                                                let detail =
                                                    jsOptions<Browser.Types.CustomEventInit>(fun o ->
                                                        o.detail <- Some (box model.Email)
                                                    )

                                                let event = Browser.Event.CustomEvent.Create("composer-editor-compose-from-reply", detail)
                                                Browser.Dom.window.dispatchEvent(event)
                                                |> ignore
                                            )
                                            buttonIcon Fa.Solid.ReplyAll (fun _ -> 
                                                let detail =
                                                    jsOptions<Browser.Types.CustomEventInit>(fun o ->
                                                        o.detail <- Some (box model.Email)
                                                    )

                                                let event = Browser.Event.CustomEvent.Create("composer-editor-compose-from-reply-all", detail)
                                                Browser.Dom.window.dispatchEvent(event)
                                                |> ignore
                                            )
                                            buttonIcon Fa.Solid.Share (fun _ -> 
                                                let detail =
                                                    jsOptions<Browser.Types.CustomEventInit>(fun o ->
                                                        o.detail <- Some (box model.Email)
                                                    )

                                                let event = Browser.Event.CustomEvent.Create("composer-editor-compose-from-transfer", detail)
                                                Browser.Dom.window.dispatchEvent(event)
                                                |> ignore
                                            )
                                        ]
                                ]

                        ]

                    div [ Class "email-media-recipients" ]
                        [ str recipients ]
                ]

            div [ Class "email-medial-body" ]
                [
                    Content.content [ ]
                        [
                            div [ DangerouslySetInnerHTML { __html = converter.makeHtml model.Email.Body } ]
                                [ ]
                        ]
                ]
        ]

let view (model : Model) (dispatch : Dispatch<Msg>) =
    div
        [
            classBaseList
                "email-media"
                [
                    "is-expanded", model.IsExpanded
                    "is-collapsed", not model.IsExpanded
                ]
            OnClick (fun ev ->
                dispatch ToggleState
            )
            Key (model.Email.Guid.ToString())
        ]
        [
            div [ Class "email-media-avatar" ]
                [
                    Image.image
                        [
                            Image.Is48x48
                        ]
                        [
                            img
                                [
                                    Src "https://bulma.io/images/placeholders/64x64.png"
                                    Class "is-rounded"
                                ]
                        ]
                ]

            div [ Class "email-media-preview" ]
                [
                    div [ Class "email-media-preview-summary" ]
                        [
                            div [ Class "email-media-sender" ]
                                [ str model.Email.From ]
                            div [ Class "prepare-truncate" ]
                                [
                                    div [ Class "email-media-preview-body" ]
                                        [ str model.Email.Body ]
                                ]
                        ]

                    div [ Class "email-media-date" ]
                        [ str (formatDate model.Email.Date) ]
                ]

            emailMediaContent model dispatch

        ]
