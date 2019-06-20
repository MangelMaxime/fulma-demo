module Mailbox.Inbox

open Fulma
open Fulma.Extensions.Wikiki
open Fable.React
open Fable.React.Props
open Fable.FontAwesome
open Elmish

type Model =
    {
        Emails : Email list
        IsLoading : bool
    }

[<RequireQualifiedAccess>]
type FetchEmailListResult =
    | Success of emails : Email list
    | Errored of exn

type Msg =
    | FetchEmailListResult of FetchEmailListResult

let private fetchInboxEmails () =
    promise {
        do! Promise.sleep 500

        return
            Database.Emails
                .sortBy("ReceivedAt")
                .value()
            |> unbox<Email []>
            |> Array.toList
            |> FetchEmailListResult.Success
    }

let init () =
    {
        Emails = []
        IsLoading = true
    }
    , Cmd.OfPromise.either fetchInboxEmails () FetchEmailListResult (FetchEmailListResult.Errored >> FetchEmailListResult)

let update (msg : Msg) (model : Model) =
    match msg with
    | FetchEmailListResult result ->
        match result with
        | FetchEmailListResult.Success emails ->
            { model with
                Emails = emails
                IsLoading = false
            }
            , Cmd.none

        | FetchEmailListResult.Errored error ->
            Logger.errorfn "Failed to retrieved the email list.\n%s" error.Message
            { model with
                IsLoading = false
            }
            , Cmd.none

let private renderEmail (email : Email) =
    let formatDate =
        Date.Format.localFormat Date.Local.englishUK "dd MMM yyyy"

    let senders =
        email.From
        |> String.concat ", "
        |> str

    Media.media [ Media.CustomClass "is-email-preview" ]
        [ Media.left [ ]
            [ Checkbox.checkbox [ ]
                [ Checkbox.input [ ]
                ]
            ]
          Media.content [ ]
            [ div [ ]
                [ str email.Subject ]
              Text.div
                [
                     Modifiers
                        [
                            Modifier.TextWeight TextWeight.Bold
                            Modifier.TextColor IsGreyDark
                            Modifier.TextSize (Screen.All, TextSize.Is7)
                        ]
                 ]
                [ senders ]
            ]
          Media.right [ ]
            [
                email.ReceivedAt
                |> formatDate
                |> str
            ]
        ]


let buttonIcon icon =
    Button.button [ ]
        [ Icon.icon [ ]
            [ Fa.i [ icon ]
                [ ]
            ]
        ]

let menubar =
    Level.level
        [ Level.Level.CustomClass "is-menubar"
          Level.Level.Modifiers [ Modifier.IsMarginless ]
        ]
        [ Level.left [ ]
            [ Button.list [ Button.List.HasAddons ]
                [ buttonIcon Fa.Solid.LongArrowAltLeft ]
              Button.list [ Button.List.HasAddons ]
                [ buttonIcon Fa.Solid.Eye
                  buttonIcon Fa.Solid.EyeSlash ]
              Button.list [ Button.List.HasAddons ]
                [ buttonIcon Fa.Regular.TrashAlt
                  buttonIcon Fa.Solid.Archive
                  buttonIcon Fa.Solid.Ban ]
            ]
          Level.right [ ]
            [ Button.list [ Button.List.HasAddons ]
                [ buttonIcon Fa.Solid.AngleLeft
                  buttonIcon Fa.Solid.AngleDown
                  buttonIcon Fa.Solid.AngleRight
                ]
            ]
        ]

let view (model : Model) (dispatch : Dispatch<Msg>) =
    let emails =
        [ for i = 0 to 20 do
            yield! model.Emails
                    |> List.map renderEmail
        ]

    div [ ]
        [ menubar
          Columns.columns [ Columns.IsGapless ]
            [ Column.column
                [ Column.CustomClass "is-email-list"
                  Column.Width (Screen.All, Column.Is5)
                ]
                // (
                //     model.Emails
                //     |> List.map renderEmail
                // )
                emails
              Column.column [ ]
                [ str (string model.IsLoading)
                ]
            ]
        ]
