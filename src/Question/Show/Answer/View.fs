module Question.Show.Answer.View

open Types
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma
open Fulma.FontAwesome
open System

let private voteArea score =
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

let root model dispatch =
    Media.media [ ]
        [ Media.left [ ]
            [ Image.image [ Image.Is64x64 ]
                [ img [ Src ("avatars/" + model.Author.Avatar) ] ] ]
          Media.content [ ]
            [ Render.contentFromMarkdown [ ] model.Answer.Content
              Level.level [ ]
                [ Level.right [ CustomClass "vote-area" ]
                    [ Button.button [ Button.IsLoading model.IsLoading
                                      Button.Size IsSmall
                                      Button.Color IsDanger
                                      Button.OnClick (fun _ -> dispatch VoteDown) ]
                        [ str "-1" ]
                      Button.button [ Button.IsLoading model.IsLoading
                                      Button.Size IsSmall
                                      Button.Color IsSuccess
                                      Button.OnClick (fun _ -> dispatch VoteUp) ]
                        [ str "+1" ]
                      voteArea model.Answer.Score
                      Help.help [ Help.Color IsDanger ]
                        [ str model.Error ] ]
                  Level.left [ ]
                    [ Level.item [ ]
                        [ Help.help [ ]
                            [ str (sprintf "Answer by %s %s, %s"
                                            model.Author.Firstname
                                            model.Author.Surname
                                            (model.Answer.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"))) ] ] ] ] ] ]
