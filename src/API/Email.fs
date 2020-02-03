module API.Email

open Fable.Core
open Helpers
open Types
open System

let fetchInboxEmails (pageRank : int) (category : Email.Category) (session : Session) =
    promise {
        do! Common.checkSessionValidity session
        do! Common.randomDelay ()

        let emails =
            Database.Emails
                .value()
            |> unbox<Email []>
            |> Array.filter (fun email ->
                match category with
                | Email.Category.Archive ->
                    email.Ancestor = None
                        && email.IsArchived
                        && not email.IsTrashed
                        && email.Type = EmailType.Received

                | Email.Category.Inbox ->
                    email.Ancestor = None
                       && not email.IsTrashed
                        && email.Type = EmailType.Received

                | Email.Category.Sent ->
                    email.Ancestor = None
                       && not email.IsTrashed
                        && email.Type = EmailType.Sent

                | Email.Category.Stared ->
                    email.Ancestor = None
                        && email.IsStared
                        && not email.IsTrashed
                        && email.Type = EmailType.Received

                | Email.Category.Trash ->
                    email.Ancestor = None
                       && email.IsTrashed
                        && email.Type = EmailType.Received

                | Email.Category.Folder(_) -> failwith "this type of request is not implemented yet"
                | Email.Category.Tag(_) -> failwith "this type of request is not implemented yet"
            )
            |> Array.toList

        let minOffset = (pageRank - 1) * 10
        let maxOffet = Math.Min(minOffset + 9, emails.Length - 1) // + 9 -> Means takins 10 elements because the index start to 0

        return
            emails.[minOffset..maxOffet]
    }

let markAsRead (guids : Guid list) (session : Session) =
    promise {
        do! Common.checkSessionValidity session
        do! Common.randomDelay ()

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

let markAsUnread (guids : Guid list) (session : Session) =
    promise {
        do! Common.checkSessionValidity session
        do! Common.randomDelay ()

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

let moveToTrask (guids : Guid list) (session : Session) =
    promise {
        do! Common.checkSessionValidity session
        do! Common.randomDelay ()

        Database.Emails
            .filter(fun (email : Email) ->
                List.contains email.Guid guids
            )
            .each(fun (email : Email) ->
                email.IsTrashed <- true
            )
            .write()

        return ()
    }

let moveToInbox (guids : Guid list) (session : Session) =
    promise {
        do! Common.checkSessionValidity session
        do! Common.randomDelay ()

        Database.Emails
            .filter(fun (email : Email) ->
                List.contains email.Guid guids
            )
            .each(fun (email : Email) ->
                email.IsTrashed <- false
            )
            .write()

        return ()
    }

type SendEmailParameters =
    {
        From : string
        To : string []
        Subject : string
        Body : string
        Tags : string []
        Ancestor : Guid option
    }

let sendEmail (parameters : SendEmailParameters) =
    promise {
        do! Common.randomDelay ()

        let errors : Validation.ErrorDef list =
            [
                if String.IsNullOrEmpty parameters.From then
                    yield
                        {
                            Key = "from"
                            Text = "This field is required"
                        }

                if Array.isEmpty parameters.To then
                    yield
                        {
                            Key = "to"
                            Text = "You need to provide at least one recipient"
                        }
            ]

        if List.isEmpty errors then
            let email =
                {
                    Guid = Guid.NewGuid()
                    From = parameters.From
                    To = parameters.To
                    Subject = parameters.Subject
                    Date = DateTime.UtcNow
                    Body = parameters.Body
                    Type = EmailType.Sent
                    IsStared = false
                    IsTrashed = false
                    IsArchived = false
                    IsRead = true
                    Tags = [| |]
                    Ancestor = parameters.Ancestor
                }

            Database.Emails
                .push(email)
                .write()

            return Ok ()

        else
            return Error errors
    }
