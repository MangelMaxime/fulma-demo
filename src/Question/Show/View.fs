module Question.Show.View

open Types
open Fable.Core
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Import
open Fulma.Extensions
open Fulma.Components
open Fulma.Elements
open Fulma.Elements.Form
open Fulma.Layouts
open System

let converter = Showdown.Globals.Converter.Create()

[<Pojo>]
type DangerousInnerHtml =
    { __html : string }

let contentFromMarkdown options str =
    Content.content
        [ yield! options
          yield Content.props [ DangerouslySetInnerHTML { __html =  converter.makeHtml str } ] ]
        [ ]

let loaderView isLoading =
    if isLoading then
        PageLoader.pageLoader [ PageLoader.isActive ]
            [ ]
    else
        PageLoader.pageLoader [ ]
            [ ]

let answerView (answer : AnswerInfo) =
    Media.media [ ]
        [ Media.left [ ]
            [ Image.image [ Image.is64x64 ]
                [ img [ Src ("avatars/" + answer.Author.Avatar) ] ] ]
          Media.content [ ]
            [ contentFromMarkdown [ ] answer.Content
              Level.level [ ]
                [ Level.right [ ]
                    [ ]
                  Level.left [ ]
                    [ Level.item [ ]
                        [ Help.help [ ]
                            [ str (sprintf "Answer by %s %s, %s"
                                        answer.Author.Firstname
                                        answer.Author.Surname
                                        (answer.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"))) ] ] ] ] ] ]

let questionsView (question : QuestionInfo) =
    Media.media [ ]
        [ Media.left [ ]
            [ Image.image [ Image.is64x64 ]
                [ img [ Src ("avatars/" + question.Author.Avatar)  ] ] ]
          Media.content [ ]
            [ yield contentFromMarkdown [ ]
                        question.Description
              yield Level.level [ ]
                        [ Level.left [ ] [ ] // Needed to force the level right aligment
                          Level.right [ ]
                            [ Level.item [ ]
                                [ Help.help [ ]
                                    [ str (sprintf "Asked by %s %s, %s"
                                                        question.Author.Firstname
                                                        question.Author.Surname
                                                        (question.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"))) ] ] ] ]
              yield! (List.map answerView question.Answers) ] ]

let pageContent (question: QuestionInfo) =
    Section.section [ ]
        [ Heading.p [ Heading.is5 ]
            [ str question.Title ]
          Columns.columns [ Columns.isCentered ]
            [ Column.column [ Column.Width.isTwoThirds ]
                [ questionsView question
                 ]
             ] ]

let root model dispatch =
    match model.State with
    | State.Error ->
        str "Something went wrong", false
    | State.Loading -> div [ ] [ ], true
    | State.Success data ->
        pageContent data, false
    |> (fun (pageContent, isLoading) ->
        Container.container [ ]
            [ loaderView isLoading
              pageContent ]
    )
