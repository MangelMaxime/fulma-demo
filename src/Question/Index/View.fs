module Question.Index.View

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
    let url =
        Router.QuestionPage.Show
        >> Router.Question
        >> Router.toHash //AuthenticatedPage.Question >> AuthPage >> toHash

    Media.media [ ]
        [ Media.left [ ]
            [ Image.image [ Image.is64x64 ]
                [ img [ Src ("avatars/" + question.Author.Avatar)  ] ] ]
          Media.content [ ]
            [ Heading.p [ Heading.isSubtitle
                          Heading.is5 ]
                [ a [ Href (url question.Id) ]
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
