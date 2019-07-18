module Mailbox.Inbox.EmailView.EmailMedia

open Fulma
open Fulma.Extensions.Wikiki
open Fable.React
open Fable.React.Props
open Fable.FontAwesome
open Elmish

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

let private emailMediaContent (model : Model) (dispatch : Dispatch<Msg>) =
    let recipients =
        model.Email.To
        |> String.concat ","

    div [ Class "email-media-content" ]
        [
            div [ Class "email-media-content-header" ]
                [
                    div [ Class "email-media-sender" ]
                        [ str model.Email.From ]

                    div [ Class "email-media-date" ]
                        [ str (formatDate model.Email.Date) ]

                    div [ Class "email-media-date" ]
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
