module Question.Show.View

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
    Media.media [ ]
        [ Media.left [ ]
            [ Image.image [ Image.is64x64 ]
                [ img [ Src ("avatars/" + question.Author.Avatar)  ] ] ]
          Media.content [ ]
            [ Content.content [ ]
                [ str question.Description ]
              Level.level [ ]
                [ Level.left [ ] [ ] // Needed to force the level right aligment
                  Level.right [ ]
                    [ Level.item [ ]
                        [ Help.help [ ]
                            [ str (sprintf "Asked by %s %s, %s"
                                                question.Author.Firstname
                                                question.Author.Surname
                                                (question.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"))) ] ] ] ] ] ]

let pageContent (data: Data) =
    Section.section [ ]
        [ Heading.p [ Heading.is5 ]
            [ str data.Question.Title ]
          Columns.columns [ Columns.isCentered ]
            [ Column.column [ Column.Width.isTwoThirds ]
                [ questionsView data.Question ]
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
