module API.User

open Fable.Core
open Helpers
open Types
open System

let signIn (email : string) (password : string) =
    promise {
        do! Common.randomDelay ()

        // Work around LowDB not working as expected...

        let user =
            Database.Users.value()
            |> unbox<User []>
            |> Array.filter (fun (user : User) ->
                user.Email = email && user.Password = password
            )
            |> Array.tryHead

        let res : Result<Session, Validation.ErrorDef list> =
            match user with
            | Some user ->
                Database.Users
                    .find(
                        {|
                            Id = user.Id
                        |} |> box
                    )
                    .assign(
                        {|
                            RefreshToken = Some ("fake-refresh-token" + Guid.NewGuid().ToString())
                        |}
                    )
                    .write()

                let user =
                    Database.Users
                        .find(
                            {|
                                Id = user.Id
                            |} |> box
                        )
                        .value()
                        |> unbox<User>

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
                Error
                    [
                        {
                            Key = "summary"
                            Text = "No user found for the given login/password association"
                        }
                    ]

        return res
    }

let logout (userId : Guid) =
    promise {
        do! Common.randomDelay()

        Database.Users
            .find(
                {|
                    Id = userId
                |}
            )
            .assign(
                {|
                    RefreshToken = None
                |}
            )
            .write()
    }
