namespace Page.Question.Show.Answer

module Component =

    open System
    open Data.Forum

    open Elmish
    open Fable.Helpers.React
    open Fable.Helpers.React.Props
    open Fulma
    open Fulma.FontAwesome

    type Model =
        { QuestionId : int
          Answer : Answer
          IsLoading : bool
          Error : string }

    type Msg =
        | VoteUp
        | VoteDown
        | VoteSuccess of int
        | VoteError of exn

    let init questionId answer =
        { QuestionId = questionId
          Answer = answer
          IsLoading = false
          Error = "" }

    let update msg (model : Model) =
        match msg with
        | VoteUp ->
            if model.IsLoading then
                model, Cmd.none
            elif model.Answer.Score = 5 then
                { model with Error = "You've already upvoted this answer 5 times, isn't that enough?" }, Cmd.none
            else
                { model with Error = ""
                             IsLoading = true }, Cmd.ofPromise
                                                    Requests.Answer.voteUp
                                                    (model.QuestionId, model.Answer.Id)
                                                    VoteSuccess
                                                    VoteError

        | VoteDown ->
            if model.IsLoading then
                model, Cmd.none
            elif model.Answer.Score = -5 then
                { model with Error = "You've already downvoted this answer 5 times, isn't that enough?" }, Cmd.none
            else
                { model with Error = ""
                             IsLoading = true }, Cmd.ofPromise
                                                    Requests.Answer.voteDown
                                                    (model.QuestionId, model.Answer.Id)
                                                    VoteSuccess
                                                    VoteError

        | VoteSuccess newScore ->
            { model with IsLoading = false
                         Answer =
                            { model.Answer with Score = newScore } }, Cmd.none

        | VoteError error ->
            Logger.errorfn "[Question.Show.Answer.State] Error when upvoting the answer: \n%O" error
            model, Cmd.none

    let private voteArea score =
        let icon =
            if score > 0 then
                Fa.I.ThumbsOUp
            else
                Fa.I.ThumbsODown

        let iterationCount =
            Math.Min(5, Math.Abs score - 1)

        span [ ]
            [ for _ in 0 .. iterationCount do
                yield Icon.faIcon [ ] [ Fa.icon icon ] ]

    let view (model : Model) dispatch =
        Media.media [ ]
            [ Media.left [ ]
                [ Image.image [ Image.Is64x64 ]
                    [ img [ Src ("avatars/" + model.Answer.Author.Avatar) ] ] ]
              Media.content [ ]
                [ Render.contentFromMarkdown [ ] model.Answer.Content
                  Level.level [ ]
                    [ Level.right [ GenericOption.CustomClass "vote-area" ]
                        [ Button.button [ yield Button.IsLoading model.IsLoading
                                          yield Button.Size IsSmall
                                          yield Button.Color IsDanger
                                          yield Button.OnClick (fun _ -> dispatch VoteDown) ]
                            [ str "-1" ]
                          Button.button [ yield Button.IsLoading model.IsLoading
                                          yield Button.Size IsSmall
                                          yield Button.Color IsSuccess
                                          yield Button.OnClick (fun _ -> dispatch VoteUp) ]
                            [ str "+1" ]
                          voteArea model.Answer.Score
                          Help.help [ Help.Color IsDanger ]
                            [ str model.Error ] ]
                      Level.left [ ]
                        [ Level.item [ ]
                            [ Help.help [ ]
                                [ str (sprintf "Answer by %s %s, %s"
                                                model.Answer.Author.Firstname
                                                model.Answer.Author.Surname
                                                (model.Answer.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"))) ] ] ] ] ] ]
