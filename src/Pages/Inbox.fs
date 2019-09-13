module Mailbox.Inbox

open Fulma
open Fulma.Extensions.Wikiki
open Fable.React
open Fable.React.Props
open Fable.FontAwesome
open Elmish
open System
open Helpers

type Model =
    {
        Emails : Map<Guid, Inbox.EmailMeta.Model>
        IsLoading : bool
        EmailView : Inbox.EmailView.Model option
    }

[<RequireQualifiedAccess>]
type FetchEmailListResult =
    | Success of emails : Email list
    | Errored of exn

type Msg =
    | FetchEmailListResult of FetchEmailListResult
    | Open of Email
    | EmailViewMsg of Inbox.EmailView.Msg
    | EmailMetaMsg of Guid * Inbox.EmailMeta.Msg

open Fable.Core.JsInterop

let private fetchInboxEmails () =
    promise {
        do! Promise.sleep 500

        let emails =
            Database.Emails
                // Temporary dynamic typing
                // Will be removed when updating the binding
                .orderBy("Date", Lowdb.Desc)
                .filter({| Ancestor = None |})
                .value()
            |> unbox<Email []>
            |> Array.toList

        return
            // Temporary limitation in order to limit the number of mails to render at one time on the screen
            // This improves the responsiveness
            emails.[..10]
            |> FetchEmailListResult.Success
    }

let init () =
    {
        Emails = Map.empty
        IsLoading = true
        EmailView = None
    }
    , Cmd.OfPromise.either fetchInboxEmails () FetchEmailListResult (FetchEmailListResult.Errored >> FetchEmailListResult)

let update (msg : Msg) (model : Model) =
    match msg with
    | FetchEmailListResult result ->
        match result with
        | FetchEmailListResult.Success emails ->
            let emails =
                emails
                |> List.map (fun email ->
                    email.Guid, Inbox.EmailMeta.init email
                )
                |> Map.ofList

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

    | Open email ->
        let (emailViewModel, emailViewCmd) = Inbox.EmailView.init email

        { model with
            EmailView = Some emailViewModel
        }
        , Cmd.map EmailViewMsg emailViewCmd

    | EmailViewMsg emailViewMsg ->
        match model.EmailView with
        | Some emailViewModel ->
            let (emailViewModel, emailViewCmd) = Inbox.EmailView.update emailViewMsg emailViewModel
            { model with
                EmailView = Some emailViewModel
            }
            , Cmd.map EmailViewMsg emailViewCmd

        | None ->
            model, Cmd.none

    | EmailMetaMsg (refGuid, emailMetaMsg) ->
        match Map.tryFind refGuid model.Emails with
        | Some emailMetaModel ->
            let (emailMetaModel, emailMetaCmd) = Inbox.EmailMeta.update emailMetaMsg emailMetaModel

            { model with
                Emails =
                    Map.add refGuid emailMetaModel model.Emails
            }
            , Cmd.map EmailMetaMsg emailMetaCmd

        | None ->
            model, Cmd.none

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

let private renderActiveEmail (email : Email) =
    div [ Class "email-view" ]
        [
            Level.level [ Level.Level.CustomClass "is-header" ]
                [
                    Level.left [ ]
                        [
                            Level.item [ ]
                                [ Heading.h4 [ ]
                                    [ str email.Subject ]
                                ]
                        ]
                    Level.right [ ]
                        [ Icon.icon [ ]
                            [
                                Fa.i
                                    [
                                        Fa.Regular.Star
                                        Fa.Size Fa.FaLarge
                                    ]
                                    [ ]
                            ]
                        ]
                ]
        ]

let view (model : Model) (dispatch : Dispatch<Msg>) =
    let emailMetaDispatch (guid : Guid) =
        Curry.apply EmailMetaMsg guid >> dispatch

    let emailList =
        model.Emails
        |> Seq.sortByDescending (fun keyValue ->
            keyValue.Value.SortableKey
        )
        |> Seq.map (fun keyValue ->
            Inbox.EmailMeta.view  keyValue.Value (emailMetaDispatch keyValue.Key)
        )
        |> Seq.toList

    div [ ]
        [
            menubar
            Columns.columns [ Columns.IsGapless ]
                [
                    Column.column
                        [
                            Column.CustomClass "is-email-list"
                            Column.Width (Screen.All, Column.Is5)
                        ]
                        emailList

                    Column.column [ Column.Width (Screen.All, Column.Is7) ]
                        [
                            model.EmailView
                            |> Option.map (fun emailViewModel ->
                                Inbox.EmailView.view emailViewModel (EmailViewMsg >> dispatch)
                            )
                            |> Option.defaultValue nothing
                        ]
                ]
        ]
