namespace Page.Question.Show

module Component =

    open Data.Forum
    open Data.User

    open Elmish
    open Fable.Core.JsInterop
    open Fable.Helpers.React
    open Fable.Helpers.React.Props
    open Fable.Import
    open Fable.PowerPack
    open Fulma

    module Answer = Answer.Component

    type Model =
        { Question : Question
          Answers : Answer.Model list
          Reply : string
          Error : string
          IsWaitingReply : bool }

    type Msg =
        | ChangeReply of string
        | Submit
        | CreateAnswerSuccess of Answer
        | CreateAnswerError of exn
        | AnswerMsg of int * Answer.Msg

    let init id =
        Requests.Question.getDetails id
        |> Promise.map (fun (question, answers) ->
            { Question = question
              Answers = Array.map (Answer.init id) answers |> Array.toList
              Reply = ""
              Error = ""
              IsWaitingReply = false } |> Ok
        )
        |> Promise.catch(fun error ->
            Error error.Message
        )

    let update currentUser msg (model: Model) =
        match msg with
        | ChangeReply value ->
            { model with Reply = value }, Cmd.none

        | Submit ->
            if model.IsWaitingReply then
                model, Cmd.none
            else
                if model.Reply <> "" then
                    { model with IsWaitingReply = true
                                 Error = "" }, Cmd.ofPromise
                                                    Requests.Answer.createAnswer
                                                    (model.Question.Id, currentUser.Id, model.Reply)
                                                    CreateAnswerSuccess
                                                    CreateAnswerError
                else
                    { model with Error = "Your answer can't be empty" }, Cmd.none

        | CreateAnswerSuccess data ->
            let answer = Answer.init model.Question.Id data
            { model with IsWaitingReply = false
                         Error = ""
                         Reply = ""
                         Answers = model.Answers @ [ answer ] }, Cmd.none

        | CreateAnswerError error ->
            Browser.console.log("An error occured when creating an answer: " + error.Message)
            { model with IsWaitingReply = false
                         Error = "An error occured, please try again" }, Cmd.none

        | AnswerMsg (refIndex, msg) ->
            let mutable newCmd = Cmd.none
            let newAnswers =
                model.Answers
                |> List.mapi(fun index answer ->
                    if index = refIndex then
                        let (subModel, subCmd) = Answer.update msg answer
                        newCmd <- Cmd.map (fun x -> AnswerMsg (index, x)) subCmd
                        subModel
                    else
                        answer
                )

            { model with Answers = newAnswers }, newCmd

    let private replyView (currentUser : User) model dispatch =
        Media.media [ ]
            [ Media.left [ ]
                [ Image.image [ Image.Is64x64 ]
                    [ img [ Src ("avatars/" + currentUser.Avatar) ] ] ]
              Media.content [ ]
                [ Field.div [ ]
                    [ Control.div [ yield Control.IsLoading model.IsWaitingReply ]
                        [ Textarea.textarea [ yield Textarea.Props [
                                                DefaultValue model.Reply
                                                Ref (fun element ->
                                                    if not (isNull element) && model.Reply = "" then
                                                        let textarea = element :?> Browser.HTMLTextAreaElement
                                                        textarea.value <- model.Reply)
                                                OnChange (fun ev -> !!ev.target?value |> ChangeReply |> dispatch)
                                                OnKeyDown (fun ev ->
                                                    if ev.ctrlKey && ev.key = "Enter" then
                                                        dispatch Submit)
                                                ]
                                              yield Textarea.Disabled model.IsWaitingReply ]
                        [ ] ]
                      Help.help [ Help.Color IsDanger ]
                                [ str model.Error ] ]
                  Level.level [ ]
                    [ Level.left [ ]
                        [ Level.item [ ]
                            [ Button.a [ yield Button.Color IsPrimary
                                         yield Button.OnClick (fun _ -> dispatch Submit)
                                         yield Button.Disabled model.IsWaitingReply ]
                                       [ str "Submit" ] ] ]
                      Level.item [ Level.Item.HasTextCentered ]
                        [ Help.help [ ]
                            [ str "You can use markdown to format your answer" ] ]
                      Level.right [ ]
                        [ Level.item [ Level.Item.Props [ Key "test" ] ]
                            [ str "Press Ctrl + Enter to submit" ] ] ] ] ]

    let private viewAnswers answers dispatch =
        div [ ]
            (answers
             |> List.mapi (fun index answer -> Answer.view answer ((fun msg -> AnswerMsg (index, msg)) >> dispatch)))

    let view currentUser model dispatch =
        Container.container [ ]
            [ Section.section [ ]
                [ Heading.p [ Heading.Is5 ]
                    [ str model.Question.Title ]
                  Columns.columns [ Columns.IsCentered ]
                    [ Column.column [ Column.Width (Screen.All, Column.Is8) ]
                        [ Views.Question.viewThread model.Question (viewAnswers model.Answers dispatch)
                          replyView currentUser model dispatch ] ] ] ]
