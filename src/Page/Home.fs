namespace Page.Home

open System

[<AutoOpen>]
module Types =

    type QuestionInfo =
        { Id : int
          Author : Database.User
          Title : string
          Description : string
          CreatedAt : DateTime }

    type Model =
        { Questions : QuestionInfo list option }

        static member Empty =
            { Questions = None }

    type GetQuestionsRes =
        | Success of QuestionInfo list
        | Error of exn

    type Msg =
        | GetQuestions
        | GetQuestionsResult of GetQuestionsRes


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
                          Author = user
                          Title = question.Title
                          Description = question.Description
                          CreatedAt = question.CreatedAt }
                )
                |> Array.toList

            do! Promise.sleep 500

            return GetQuestionsRes.Success result
        }

module State =

    open Elmish

init session slug =
    let
        maybeAuthToken =
            Maybe.map .token session.user

        loadArticle =
            Request.Article.get maybeAuthToken slug
                |> Http.toTask

        loadComments =
            Request.Article.Comments.list maybeAuthToken slug
                |> Http.toTask

        handleLoadError _ =
            pageLoadError Page.Other "Article is currently unavailable."
    in
    Task.map2 (Model [] "" False) loadArticle loadComments
        |> Task.mapError handleLoadError

    open System
    open Fable.Core
    open Fable.Import
    open Fable.Core.JsInterop

    let init session =
        { Questions = [] }, Cmd.ofPromise Request.getQuestions () GetQuestionsResult (GetQuestionsRes.Error >> GetQuestionsResult)

    let update msg (model: Model) =
        match msg with
        | GetQuestions ->
            model, Cmd.ofPromise Request.getQuestions () GetQuestionsResult (GetQuestionsRes.Error >> GetQuestionsResult)

        | GetQuestionsResult result ->
            match result with
            | GetQuestionsRes.Success questions ->
                { model with Questions = Some questions }, Cmd.none

            | GetQuestionsRes.Error error ->
                Logger.debugfn "[Question.Index.State] Error when fetch questions:\n %A" error
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

    let loaderView isLoading =
        if isLoading then
            PageLoader.pageLoader [ PageLoader.isActive ]
                [ ]
        else
            PageLoader.pageLoader [ ]
                [ ]

    let questionsView (question : QuestionInfo) =
        let urlToQuestion =
            Router.QuestionPage.Show
            >> Router.Question //AuthenticatedPage.Question >> AuthPage >> toHash

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
                                [ str (sprintf "Asked by %s %s, %s"
                                                    question.Author.Firstname
                                                    question.Author.Surname
                                                    (question.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"))) ] ] ] ] ] ]

    let questionsList questions =
        Columns.columns [ Columns.isCentered ]
            [ Column.column [ Column.Width.isTwoThirds ]
                (questions |> List.map questionsView) ]

    let root model dispatch =
        match model.Questions with
        | Some questions ->
            Container.container [ ]
                [ loaderView false
                  Section.section [ ]
                    [ Heading.h3 [ ]
                        [ str "Latest questions" ] ]
                  questionsList questions ]
        | None ->
            Container.container [ ]
                [ loaderView true ]
