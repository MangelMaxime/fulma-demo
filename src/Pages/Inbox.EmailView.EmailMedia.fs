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
    | Expand
    | Collaspe

let init (email : Email) =
    {
        IsExpanded = false
        Email = email
    }
    , Cmd.none

let update (msg : Msg) (model : Model) =
    match msg with
    | Expand ->
        { model with
            IsExpanded = true
        }
        , Cmd.none

    | Collaspe ->
        { model with
            IsExpanded = false
        }
        , Cmd.none


let view (model : Model) (dispatch : Dispatch<Msg>) =
    let formatDate =
        Date.Format.localFormat Date.Local.englishUK "ddd dd/MM/yyyy hh:mm"

    div [ Class "email-media" ]
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
                            div [ Class "email-media-preview-sender" ]
                                [ str model.Email.From ]
                            div [ Class "prepare-truncate" ]
                                [
                                    div [ Class "email-media-preview-body" ]
                                        [ str model.Email.Body ]
                                ]
                        ]

                    div [ Class "email-media-preview-date" ]
                        [ str (formatDate model.Email.Date) ]
                ]
        ]
