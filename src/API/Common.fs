module API.Common

open Types
open System
open Helpers
open Fable.Core
open Fable.Core.JsInterop

exception ExpiredSession

let checkSessionValidity (session : Session) =
    promise {
        if session.SimpleJWT < DateTime.UtcNow then
            raise ExpiredSession
    }

let randomDelay () =
    promise {
        do! Promise.sleep (int (Random.between 500. 1200.))
    }

let private refreshJWTToken (token : string) =
    promise {
        let newToken = ("fake-refresh-token" + Guid.NewGuid().ToString())
        let user =
            Database.Users
                .find(fun (user : User) ->
                    user.RefreshToken = Some token
                )
                .assign(
                    {|
                        RefreshToken = Some newToken
                    |}
                )

        user.write()

        // For some reason, when accessing the user after the write it lose all of the nnon assign fields...
        // We will remove the lowdb dependency and handle everything by hand because it keeps getting in the way again and again
        let user =
            Database.Users
                .find(fun (user : User) ->
                    user.RefreshToken = Some newToken
                )

        let res =
            match Lowdb.tryValue<User> user with
            | Some user ->
                Ok
                    {
                        UserId = user.Id
                        Firstname = user.Firstname
                        Surname = user.Surname
                        Email = user.Email
                        SimpleJWT = DateTime.UtcNow.AddSeconds(5.)
                        RefreshToken = user.RefreshToken.Value
                    }
            | None ->
                Error "No user found for the given refresh token"

        return res
    }
