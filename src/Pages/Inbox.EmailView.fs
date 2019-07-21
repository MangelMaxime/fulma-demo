module Mailbox.Inbox.EmailView

open Fulma
open Fulma.Extensions.Wikiki
open Fable.React
open Fable.React.Props
open Fable.FontAwesome
open Elmish
open System

type Model =
    {
        IsLoading : bool
        Email : Email
        History : Map<Guid, EmailView.EmailMedia.Model>
    }

[<RequireQualifiedAccess>]
type FetchEmailHistoryResult =
    | Success of emails : Email list
    | Errored of exn

type Msg =
    | FetchEmailHistoryResult of FetchEmailHistoryResult
    | EmailMediaMsg of Guid * EmailView.EmailMedia.Msg

let private fetchEmailHistory (email : Email) =
    promise {
        do! Promise.sleep 400

        // It's working but that's not really pretty code ¯\_(ツ)_/¯

        let getEmailAnswers (ancestor : Email) : Email list =
            Database.Emails
                .filter({| Ancestor = Some ancestor.Guid |})
                .value()
            |> unbox<Email []>
            |> Array.toList

        let rec apply (toHandle : Email list) (result : Email list) =
            match toHandle with
            | head::tail ->
                let localhistory = getEmailAnswers head
                apply (tail @ localhistory) (result @ localhistory)

            | [ ] ->
                result

        return
            apply [ email ] [ email ]
            |> List.sortBy (fun email ->
                email.Date
            )
            |> FetchEmailHistoryResult.Success
    }

let init (email : Email) =
    {
        IsLoading = true
        Email = email
        History = Map.empty
    }
    , Cmd.OfPromise.either fetchEmailHistory email FetchEmailHistoryResult (FetchEmailHistoryResult.Errored >> FetchEmailHistoryResult)

let update (msg : Msg) (model : Model) =
    match msg with
    | FetchEmailHistoryResult result ->
        match result with
        | FetchEmailHistoryResult.Success history ->
            let history, cmds =
                history
                |> List.map (fun email ->
                    let (emailMediaModel, emailMediaCmd) = EmailView.EmailMedia.init email
                    (email.Guid, emailMediaModel), Cmd.mapWithGuid EmailMediaMsg email.Guid emailMediaCmd
                )
                |> List.fold (fun (models, cmds) modelAndCmd ->
                    fst modelAndCmd :: models, snd modelAndCmd :: cmds
                ) ([], [])


            { model with
                IsLoading = false
                History = Map.ofList history
            }
            , Cmd.batch cmds

        | FetchEmailHistoryResult.Errored error ->
            Logger.errorfn "Failed to retrieved the email history.\n%s" error.Message
            model, Cmd.none

    | EmailMediaMsg (guid, emailMediaMsg) ->
        match Map.tryFind guid model.History with
        | Some emailMediaModel ->
            let (emailMediaModel, emailMediaCmd) = EmailView.EmailMedia.update emailMediaMsg emailMediaModel
            { model with
                History = Map.add guid emailMediaModel model.History
            }
            , Cmd.mapWithGuid EmailMediaMsg guid emailMediaCmd

        | None ->
            model, Cmd.none


let view (model : Model) (dispatch : Dispatch<Msg>) =
    div [ Class "email-view" ]
        [
            Level.level [ Level.Level.CustomClass "is-header" ]
                [
                    Level.left [ ]
                        [
                            Level.item [ ]
                                [ Heading.h4 [ ]
                                    [ str model.Email.Subject ]
                                ]
                        ]
                    // Level.right [ ]
                    //     [ Icon.icon [ ]
                    //         [
                    //             Fa.i
                    //                 [
                    //                     Fa.Regular.Star
                    //                     Fa.Size Fa.FaLarge
                    //                 ]
                    //                 [ ]
                    //         ]
                    //     ]
                ]
            Map.toList model.History
            |> List.sortBy (fun (guid, emailMediaModel) ->
                emailMediaModel.Email.Date
            )
            |> List.map (fun (guid, emailMediaModel) ->
                let dispatch =
                    (fun msg -> dispatch (EmailMediaMsg (guid, msg)) )
                EmailView.EmailMedia.view emailMediaModel dispatch
            )
            |> ofList
        ]
