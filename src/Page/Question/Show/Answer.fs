namespace Page.Question.Show.Answer

module Component =

    open Data.Forum

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

    open Elmish

    let update msg (model: Model) =
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

    open Fulma.Layouts
    open Fulma.Components
    open Fulma.Elements
    open Fulma.Elements.Form
    open Fulma.Extra.FontAwesome
    open Fable.Helpers.React
    open Fable.Helpers.React.Props
    open System

    let voteArea score =
        let icon =
            if score > 0 then
                Fa.I.ThumbsOUp
            else
                Fa.I.ThumbsODown

        let iterationCount =
            Math.Min(5, Math.Abs score - 1)

        span [ ]
            [ for i = 0 to iterationCount do
                yield Icon.faIcon [ ] [ Fa.icon icon ] ]

    let view (model : Model) dispatch =
        Media.media [ ]
            [ Media.left [ ]
                [ Image.image [ Image.is64x64 ]
                    [ img [ Src ("avatars/" + model.Answer.Author.Avatar) ] ] ]
              Media.content [ ]
                [ Render.contentFromMarkdown [ ] model.Answer.Content
                  Level.level [ ]
                    [ Level.right [ Level.customClass "vote-area" ]
                        [ Button.button_btn [ if model.IsLoading then
                                                yield Button.isLoading
                                              yield Button.isSmall
                                              yield Button.isDanger
                                              yield Button.onClick (fun _ -> dispatch VoteDown) ]
                            [ str "-1" ]
                          Button.button_btn [ if model.IsLoading then
                                                yield Button.isLoading
                                              yield Button.isSmall
                                              yield Button.isSuccess
                                              yield Button.onClick (fun _ -> dispatch VoteUp) ]
                            [ str "+1" ]
                          voteArea model.Answer.Score
                          Help.help [ Help.isDanger ]
                            [ str model.Error ] ]
                      Level.left [ ]
                        [ Level.item [ ]
                            [ Help.help [ ]
                                [ str (sprintf "Answer by %s %s, %s"
                                                model.Answer.Author.Firstname
                                                model.Answer.Author.Surname
                                                (model.Answer.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"))) ] ] ] ] ] ]
