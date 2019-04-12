module Question.Show.View

open Types
open Fable.React
open Fable.React.Props
open Fulma
open Fulma.Extensions.Wikiki
open Fable.Core.JsInterop

let private loaderView isLoading =
    PageLoader.pageLoader [ PageLoader.IsActive isLoading ]
        [ ]

let private replyView user model dispatch =
    Media.media [ ]
        [ Media.left [ ]
            [ Image.image [ Image.Is64x64 ]
                [ img [ Src ("avatars/" + user.Avatar) ] ] ]
          Media.content [ ]
            [ Field.div [ ]
                [ Control.div [ Control.IsLoading model.IsWaitingReply ]
                    [ Textarea.textarea [ Textarea.Props [
                                            DefaultValue model.Reply
                                            Ref (fun element ->
                                                if not (isNull element) && model.Reply = "" then
                                                    let textarea = element :?> Browser.Types.HTMLTextAreaElement
                                                    textarea.value <- model.Reply
                                            )
                                            OnChange (fun ev -> !!ev.target?value |> ChangeReply |> dispatch)
                                            OnKeyDown (fun ev ->
                                                if ev.ctrlKey && ev.key = "Enter" then
                                                    dispatch Submit
                                            ) ]
                                          Textarea.Disabled model.IsWaitingReply ]
                    [ ] ]
                  Help.help [ Help.Color IsDanger ]
                            [ str model.Error ] ]
              Level.level [ ]
                [ Level.left [ ]
                    [ Level.item [ ]
                        [ Button.button [ Button.Color IsPrimary
                                          Button.OnClick (fun _ -> dispatch Submit)
                                          Button.Disabled model.IsWaitingReply ]
                                        [ str "Submit" ] ] ]
                  Level.item [ Level.Item.HasTextCentered ]
                    [ Help.help [ ]
                        [ str "You can use markdown to format your answer" ] ]
                  Level.right [ ]
                    [ Level.item [ ]
                        [ str "Press Ctrl + Enter to submit" ] ] ] ] ]

let private questionsView (question : QuestionInfo) answers dispatch =
    Media.media [ ]
        [ Media.left [ ]
            [ Image.image [ Image.Is64x64 ]
                [ img [ Src ("avatars/" + question.Author.Avatar)  ] ] ]
          Media.content [ ]
            [ yield Answer.View.contentFromMarkdown [ ]
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

let private pageContent user question model dispatch =
    Section.section [ ]
        [ Heading.p [ Heading.Is5 ]
            [ str question.Title ]
          questionsView question model.Answers dispatch
          replyView user model dispatch ]

let root (props: {| user: User
                    model: Model
                    dispatch: Msg->unit |}) =
    match props.model.Question with
    | Some question ->
        pageContent props.user question props.model props.dispatch, false
    | None -> div [ ] [ ], true
    |> (fun (pageContent, isLoading) ->
        Container.container [ ]
            [ loaderView isLoading
              pageContent ]
    )
