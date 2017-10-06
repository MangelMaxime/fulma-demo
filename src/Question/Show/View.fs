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

let replyView (user: User) (fieldValue: StringField) isWaiting dispatch =
    Media.media [ ]
        [ Media.left [ ]
            [ Image.image [ Image.is64x64 ]
                [ img [ Src ("avatars/" + user.Avatar) ] ] ]
          Media.content [ ]
            [ Field.field_div [ ]
                [ yield Control.control_div [ if isWaiting then yield Control.isLoading ]
                    [ Textarea.textarea [ yield Textarea.props [
                                            Value fieldValue.Value
                                            OnChange (fun ev -> !!ev.target?value |> ChangeReply |> dispatch)
                                            OnKeyDown (fun ev ->
                                                if ev.ctrlKey && ev.key = "Enter" then
                                                    dispatch Submit
                                            ) ]
                                          if isWaiting then yield Textarea.isDisabled ]
                    [ ] ]
                  if fieldValue.Error.IsSome then
                    yield Help.help [ Help.isDanger ]
                            [ str fieldValue.Error.Value ] ]
              Level.level [ ]
                [ Level.left [ ]
                    [ Level.item [ ]
                        [ Button.button [ yield Button.isPrimary
                                          yield Button.onClick (fun _ -> dispatch Submit)
                                          if isWaiting then yield Button.isDisabled ]
                                        [ str "Submit" ] ] ]
                  Level.right [ ]
                    [ Level.item [ ]
                        [ str "Press Ctrl + Enter to submit" ] ] ] ] ]

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

let pageContent (user: User) (question: QuestionInfo) reply isWaitingReply dispatch =
    Section.section [ ]
        [ Heading.p [ Heading.is5 ]
            [ str question.Title ]
          Columns.columns [ Columns.isCentered ]
            [ Column.column [ Column.Width.isTwoThirds ]
                [ questionsView question
                  replyView user reply isWaitingReply dispatch ] ] ]

let root model dispatch =
    match model.Data with
    | Some data ->
        Logger.debug "some"
        pageContent model.Session data.QuestionInfo model.Reply model.IsWaitingReply dispatch, false
    | None -> div [ ] [ ], true
    |> (fun (pageContent, isLoading) ->
        Logger.debug "none"
        Container.container [ ]
            [ loaderView isLoading
              pageContent ]
    )
