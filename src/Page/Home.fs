namespace Page.Home

open System
open Data.Question

[<AutoOpen>]
module Types =

    type Model =
        { Questions : Question list }

    type Msg =
        | GetQuestions

module Request =

    open Fable.PowerPack
    open Types
    open Database
    open Fable.Core.JsInterop

    let getQuestions _ =
        promise {

            let result =
                Database.Questions
                    .sortBy("Id")
                    .value()
                |> unbox<Database.Question []>
                |> Array.map(fun question ->
                    match Database.GetUserById question.AuthorId with
                    | None -> failwithf "Unkown author of id#%i for the question#%i" question.AuthorId question.Id
                    | Some user ->
                        { Id = question.Id
                          Author =
                            { Id = user.Id
                              Firstname = user.Firstname
                              Surname = user.Surname
                              Avatar = user.Avatar }
                          Title = question.Title
                          Description = question.Description
                          CreatedAt = question.CreatedAt }
                )
                |> Array.toList
                |> toJson

            do! Promise.sleep 500

            return result
        }

module State =

    open Elmish
    open Fable.Import
    open Fable.PowerPack
    open Fable.PowerPack.Result
    open Fable.PowerPack.Json
    open Json.Parser
    open Views

    let init session =
        Request.getQuestions ()
        |> Promise.map (fun result ->
            try
                let json =
                    result
                    |> ofString
                    |> Result.bind array
                    |> unwrapResult

                { Questions = json
                              |> Array.map (fun item -> object item |> unwrapResult |> Question.Decoder)
                              |> Array.toList } |> Ok
            with
                | ex ->
                    Browser.console.log ex.Message
                    Page.Errored.State.pageLoadError Page.Other "Homepage is currently unavailable."
                    |> Error
        )

    let update session msg model =
        model, Cmd.none

module View =

    open Types
    open Fable.Helpers.React
    open Fable.Helpers.React.Props
    open Fulma.Extensions
    open Fulma.Components
    open Fulma.Elements
    open Fulma.Elements.Form
    open Fulma.Layouts
    open System

    let questionsView (question : Question) =
        let urlToQuestion =
            Router.QuestionPage.Show
            >> Router.Question //AuthenticatedPage.Question >> AuthPage >> toHash

        let askedBy firstname surname (createdAt: DateTime) =
            let createdAtStr = createdAt.ToString("yyyy-MM-dd HH:mm:ss")
            "Asked by " + firstname + " " + surname + ", " + createdAtStr

        Media.media [ ]
            [ Media.left [ ]
                [ Image.image [ Image.is64x64 ]
                    [ img [ Src ("avatars/" + question.Author.Avatar)  ] ] ]
              Media.content [ ]
                [ Heading.p [ Heading.isSubtitle
                              Heading.is5 ]
                    [ a [ Router.href (urlToQuestion question.Id) ]
                        [ str question.Title ] ]
                  Level.level [ ]
                    [ Level.left [ ] [ ] // Needed to force the level right aligment
                      Level.right [ ]
                        [ Level.item [ ]
                            [ Help.help [ ]
                                [ str (askedBy question.Author.Firstname
                                               question.Author.Surname
                                               question.CreatedAt) ] ] ] ] ] ]

    let questionsList questions =
        Columns.columns [ Columns.isCentered ]
            [ Column.column [ Column.Width.isTwoThirds ]
                (questions |> List.map questionsView) ]

    let root session model dispatch =
        Container.container [ ]
            [ Section.section [ ]
                [ Heading.h3 [ ]
                    [ str "Latest questions" ] ]
              questionsList model.Questions ]
