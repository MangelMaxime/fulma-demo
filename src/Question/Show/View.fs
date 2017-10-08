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
open Fable.Core.JsInterop

let loaderView isLoading =
    if isLoading then
        PageLoader.pageLoader [ PageLoader.isActive ]
            [ ]
    else
        PageLoader.pageLoader [ ]
            [ ]

let replyView model dispatch =
    Media.media [ ]
        [ Media.left [ ]
            [ Image.image [ Image.is64x64 ]
                [ img [ Src ("avatars/" + model.Session.Avatar) ] ] ]
          Media.content [ ]
            [ Field.field_div [ ]
                [ Control.control_div [ if model.IsWaitingReply then yield Control.isLoading ]
                    [ Textarea.textarea [ yield Textarea.props [
                                            Value model.Reply
                                            OnChange (fun ev -> !!ev.target?value |> ChangeReply |> dispatch)
                                            OnKeyDown (fun ev ->
                                                if ev.ctrlKey && ev.key = "Enter" then
                                                    dispatch Submit
                                            ) ]
                                          if model.IsWaitingReply then yield Textarea.isDisabled ]
                    [ ] ]
                  Help.help [ Help.isDanger ]
                            [ str model.Error ] ]
              Level.level [ ]
                [ Level.left [ ]
                    [ Level.item [ ]
                        [ Button.button [ yield Button.isPrimary
                                          yield Button.onClick (fun _ -> dispatch Submit)
                                          if model.IsWaitingReply then yield Button.isDisabled ]
                                        [ str "Submit" ] ] ]
                  Level.item [ Level.Item.hasTextCentered ]
                    [ Help.help [ ]
                        [ str "You can use markdown to format your answer" ] ]
                  Level.right [ ]
                    [ Level.item [ ]
                        [ str "Press Ctrl + Enter to submit" ] ] ] ] ]

let questionsView (question : QuestionInfo) answers dispatch =
    Media.media [ ]
        [ Media.left [ ]
            [ Image.image [ Image.is64x64 ]
                [ img [ Src ("avatars/" + question.Author.Avatar)  ] ] ]
          Media.content [ ]
            [ yield Render.contentFromMarkdown [ ]
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
              yield! (
                  answers
                  |> List.mapi (fun index answer -> Answer.View.root answer ((fun msg -> AnswerMsg (index, msg)) >> dispatch))) ] ]

let pageContent question model dispatch =
    Section.section [ ]
        [ Heading.p [ Heading.is5 ]
            [ str question.Title ]
          Columns.columns [ Columns.isCentered ]
            [ Column.column [ Column.Width.isTwoThirds ]
                [ questionsView question model.Answers dispatch
                  replyView model dispatch ] ] ]

let root model dispatch =
    match model.Question with
    | Some question ->
        pageContent question model dispatch, false
    | None -> div [ ] [ ], true
    |> (fun (pageContent, isLoading) ->
        Container.container [ ]
            [ loaderView isLoading
              pageContent ]
    )
