module Question.Index.View

open Types
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma.Extensions
open Fulma.Components
open Fulma.Elements
open Fulma.Elements.Form
open Fulma.Layouts

let loaderView isLoading =
    PageLoader.pageLoader [ PageLoader.IsActive isLoading ]
        [ ]

let questionsView (question : QuestionInfo) =
    let url =
        Router.QuestionPage.Show
        >> Router.Question
        >> Router.toHash //AuthenticatedPage.Question >> AuthPage >> toHash

    Media.media [ ]
        [ Media.left [ ]
            [ Image.image [ Image.Is64x64 ]
                [ img [ Src ("avatars/" + question.Author.Avatar)  ] ] ]
          Media.content [ ]
            [ Heading.p [ Heading.IsSubtitle
                          Heading.Is5 ]
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
    Columns.columns [ Columns.IsCentered ]
        [ Column.column [ Column.Width(Column.All, Column.IsTwoThirds) ]
            (questions |> List.map questionsView) ]

let root model _ =
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
