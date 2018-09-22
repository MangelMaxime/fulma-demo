module Views.Question

open Data.Forum
open System

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma

let private viewFooter firstname surname (createdAt : DateTime) =
    let createdAtStr = createdAt.ToString("yyyy-MM-dd HH:mm:ss")

    Level.level [ ]
        [ Level.left [ ] [ ] // Needed to force the level right aligment
          Level.right [ ]
            [ Level.item [ ]
                [ Help.help [ ]
                    [ str ("Asked by " + firstname + " " + surname + ", " + createdAtStr) ] ] ] ]

let private viewContent questionId questionTitle =
    let urlToQuestion =
        Router.QuestionPage.Show
        >> Router.Question

    Heading.p [ Heading.IsSubtitle
                Heading.Is5 ]
            [ a [ Router.href (urlToQuestion questionId) ]
                [ str questionTitle ] ]

let viewSummary (question : Question) =
    Media.media [ ]
        [ Media.left [ ]
            [ Assets.avatar64x64 question.Author.Avatar ]
          Media.content [ ]
            [ viewContent question.Id question.Title
              viewFooter question.Author.Firstname question.Author.Surname question.CreatedAt ] ]

let viewThread (question : Question) content =
    Media.media [ ]
        [ Media.left [ ]
            [ Assets.avatar64x64 question.Author.Avatar ]
          Media.content [ ]
            [ Render.contentFromMarkdown [ ]
                question.Description
              viewFooter question.Author.Firstname question.Author.Surname question.CreatedAt
              content ] ]
