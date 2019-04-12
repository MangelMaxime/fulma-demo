module Question.Show.Answer.View

open Types
open Fable.React
open Fable.React.Props
open Fulma
open Fable.FontAwesome
open Fable.FontAwesome.Free
open System

let converter = Showdown.Globals.Converter.Create()

type DangerousInnerHtml =
    { __html : string }

let contentFromMarkdown options str =
    Content.content
        [ yield! options
          yield Content.Props [ DangerouslySetInnerHTML { __html =  converter.makeHtml str } ] ]
        [ ]

let private voteArea score =
    let icon =
        if score > 0 then
            Fa.Regular.ThumbsUp
        else
            Fa.Regular.ThumbsDown

    let iterationCount =
        Math.Min(5, Math.Abs score - 1)

    span [ ]
        [ for i = 0 to iterationCount do
            yield Icon.icon [ ]
                    [ Fa.i [ icon ] [ ] ] ]

let root model dispatch =
    Media.media [ ]
        [ Media.left [ ]
            [ Image.image [ Image.Is64x64 ]
                [ img [ Src ("avatars/" + model.Author.Avatar) ] ] ]
          Media.content [ ]
            [ contentFromMarkdown [ ] model.Answer.Content
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
