module API.Email

open Fable.Core
open Helpers
open Types
open System

let fetchInboxEmails (pageRank : int) (category : Email.Category) =
    promise {
        do! Promise.sleep (int (Random.between 500. 1200.))

        let filter =
            match category with
            | Email.Category.Archive ->
                {|
                    Ancestor = None
                    IsArchived = true
                    IsTrashed = false
                    Type = EmailType.Received
                |} |> box
            | Email.Category.Inbox ->
                {|
                    Ancestor = None
                    IsTrashed = false
                    Type = EmailType.Received
                |} |> box
            | Email.Category.Sent ->
                {|
                    Ancestor = None
                    IsTrashed = false
                    Type = EmailType.Sent
                |} |> box
            | Email.Category.Stared ->
                {|
                    Ancestor = None
                    IsStared = false
                    IsTrashed = false
                    Type = EmailType.Received
                |} |> box
            | Email.Category.Trash ->
                {|
                    Ancestor = None
                    IsTrashed = false
                    Type = EmailType.Received
                |} |> box
            | Email.Category.Folder(_) -> failwith "this type of request is not implemented yet"
            | Email.Category.Tag(_) -> failwith "this type of request is not implemented yet"

        let emails =
            Database.Emails
                .orderBy("Date", Lowdb.Desc)
                .filter(filter)
                .value()
            |> unbox<Email []>
            |> Array.toList

        let minOffset = (pageRank - 1) * 10
        let maxOffet = Math.Min(minOffset + 10, emails.Length - 1)

        return
            // Temporary limitation in order to limit the number of mails to render at one time on the screen
            // This improves the responsiveness
            emails.[minOffset..maxOffet]
    }

let markAsRead (guids : Guid list) =
    promise {
        Database.Emails
            .filter(fun (email : Email) ->
                List.contains email.Guid guids
            )
            .each(fun (email : Email) ->
                email.IsRead <- true
            )
            .write()

        let updatedEmails =
            Database.Emails
                .filter(fun (email : Email) ->
                    List.contains email.Guid guids
                )
                .value()
            |> unbox<Email []>
            |> Array.toList

        return updatedEmails
    }

let markAsUnread (guids : Guid list) =
    promise {
        Database.Emails
            .filter(fun (email : Email) ->
                List.contains email.Guid guids
            )
            .each(fun (email : Email) ->
                email.IsRead <- false
            )
            .write()

        let updatedEmails =
            Database.Emails
                .filter(fun (email : Email) ->
                    List.contains email.Guid guids
                )
                .value()
            |> unbox<Email []>
            |> Array.toList

        return updatedEmails
    }
