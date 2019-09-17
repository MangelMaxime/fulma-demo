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
        PageRank : int
        Emails : Map<Guid, Inbox.EmailMeta.Model>
        CheckedEmails : Set<Guid>
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
    | ToggleCheck of Guid
    | DeselectAll
    | EmailViewMsg of Inbox.EmailView.Msg
    | EmailMetaMsg of Guid * Inbox.EmailMeta.Msg

open Fable.Core.JsInterop

let private fetchInboxEmails pageRank =
    promise {
        do! Promise.sleep (int (Random.between 500. 1200.))

        let emails =
            Database.Emails
                // Temporary dynamic typing
                // Will be removed when updating the binding
                .orderBy("Date", Lowdb.Desc)
                .filter({| Ancestor = None |})
                .value()
            |> unbox<Email []>
            |> Array.toList

        let offset = (pageRank - 1) * 10

        return
            // Temporary limitation in order to limit the number of mails to render at one time on the screen
            // This improves the responsiveness
            emails.[offset..offset+10]
            |> FetchEmailListResult.Success
    }

let init (pageRank : int) =
    {
        PageRank = pageRank
        Emails = Map.empty
        CheckedEmails = Set.empty
        IsLoading = true
        EmailView = None
    }
    , Cmd.OfPromise.either fetchInboxEmails pageRank FetchEmailListResult (FetchEmailListResult.Errored >> FetchEmailListResult)

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
            let (emailMetaModel, emailMetaCmd, externalMsg) = Inbox.EmailMeta.update emailMetaMsg emailMetaModel

            let (model, extraCmd) =
                match externalMsg with
                | Inbox.EmailMeta.ExternalMsg.NoOp ->
                    model
                    , Cmd.none

                | Inbox.EmailMeta.ExternalMsg.Selected selectedEmail ->
                    { model with
                        CheckedEmails = Set.empty
                    }
                    , Cmd.ofMsg (Open selectedEmail)

            { model with
                Emails =
                    Map.add refGuid emailMetaModel model.Emails
            }
            , Cmd.batch
                [
                    Cmd.map EmailMetaMsg emailMetaCmd
                    extraCmd
                ]

        | None ->
            model, Cmd.none

    | ToggleCheck guid ->
        { model with
            CheckedEmails = Set.toggle guid model.CheckedEmails
        }
        , Cmd.none

    | DeselectAll ->
        { model with
            CheckedEmails = Set.empty
        }
        , Cmd.none

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

let private renderCheckedEmailsView (model : Model) (dispatch : Dispatch<Msg>) =
    let text =
        sprintf "%i conversations selected" (Set.count model.CheckedEmails)

    let summaryItem =
        div [ Class "checked-summary-item" ]

    let actionButton =
        if Set.count model.CheckedEmails > 0 then
            Button.button
                [
                    Button.Color IsPrimary
                    Button.OnClick (fun _ ->
                        dispatch DeselectAll
                    )
                ]
                [ str "Deselect all" ]
        else
            nothing

    div [ Class "checked-summary" ]
        [
            summaryItem
                [
                    Icon.icon [ Icon.Size IsMedium ]
                        [
                            Fa.i
                                [
                                     Fa.Solid.CheckSquare
                                     Fa.Size Fa.Fa2x
                                ]
                                [ ]
                        ]
                ]
            summaryItem
                [
                    Text.div
                        [
                            Modifiers
                                [
                                    Modifier.TextSize (Screen.All, TextSize.Is5)
                                    Modifier.TextColor IsGrey
                                ]
                        ]
                        [ str text ]
                ]
            summaryItem
                [
                    actionButton
                ]
        ]

let view (model : Model) (dispatch : Dispatch<Msg>) =
    let emailMetaDispatch (guid : Guid) =
        Curry.apply EmailMetaMsg guid >> dispatch

    let emailList =
        if model.IsLoading then
            div [ Class "loader-dual-ring-container" ]
                [
                    div [ Class "loader-dual-ring is-medium is-black" ]
                        [ ]
                ]
        else
            model.Emails
            |> Seq.sortByDescending (fun keyValue ->
                keyValue.Value.SortableKey
            )
            |> Seq.map (fun keyValue ->
                Inbox.EmailMeta.view
                    {
                        Model = keyValue.Value
                        IsChecked = Set.contains keyValue.Key model.CheckedEmails
                        OnCheck = ToggleCheck >> dispatch
                        Dispatch = (emailMetaDispatch keyValue.Key)
                    }

            )
            |> Seq.toList
            |> ofList

    let rightColumnContent =
        match model.EmailView with
        | Some emailViewModel ->
            // Only render the emailView if we have no selection
            if Set.count model.CheckedEmails > 0 then
                renderCheckedEmailsView model dispatch
            else
                Inbox.EmailView.view emailViewModel (EmailViewMsg >> dispatch)

        | None ->
            renderCheckedEmailsView model dispatch


    div [ Class "inbox-container" ]
        [
            menubar
            Columns.columns [ Columns.IsGapless ]
                [
                    Column.column
                        [
                            Column.CustomClass "is-email-list"
                            Column.Width (Screen.All, Column.Is5)
                        ]
                        [ emailList ]

                    Column.column [ Column.Width (Screen.All, Column.Is7) ]
                        [
                            rightColumnContent
                        ]
                ]
        ]
