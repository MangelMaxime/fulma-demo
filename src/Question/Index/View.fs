module Question.Index.View

open Types
open Fable.React
open Fable.React.Props
open Fulma
open Fulma.Extensions.Wikiki

let private loaderView isLoading =
    PageLoader.pageLoader [ PageLoader.IsActive isLoading ]
        [ ]

let private questionsView (question : QuestionInfo) =
    let url =
        Router.QuestionPage.Show
        >> Router.Question

    Media.media [ ]
        [ Media.left [ ]
            [ Image.image [ Image.Is64x64 ]
                [ img [ Src ("avatars/" + question.Author.Avatar)  ] ] ]
          Media.content [ ]
            [ Heading.p [ Heading.IsSubtitle
                          Heading.Is5 ]
                [ a [ Router.href (url question.Id) ]
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

let private questionsList questions =
    Columns.columns [ Columns.IsCentered ]
        [ Column.column [ Column.Width(Screen.All, Column.IsTwoThirds) ]
            (questions |> List.map questionsView) ]

let root model _ =
    match model.Questions with
    | Some questions ->
        Container.container [ ]
            [ loaderView false
              Section.section [ ]
                [ Columns.columns [ ]
                    [ Column.column [ Column.Width(Screen.All, Column.IsNarrow) ]
                        [ Heading.h3 [ ]
                            [ str "Latest questions" ] ]
                      Column.column [ ] [ ]
                      Column.column [ Column.Width (Screen.All, Column.IsNarrow) ]
                        [ Button.a [ Button.Color IsPrimary
                                     Button.Props [ Router.href (Router.Question Router.Create) ] ]
                            [ str "Ask a new question" ] ] ] ]
              questionsList questions ]
    | None ->
        Container.container [ ]
            [ loaderView true ]
