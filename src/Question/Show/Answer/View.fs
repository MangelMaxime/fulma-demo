module Question.Show.Answer.View

open Types
open Fable.Core
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Import
open Fulma.Extensions
open Fulma.Components
open Fulma.Extra.FontAwesome
open Fulma.Elements
open Fulma.Elements.Form
open Fulma.Layouts
open System
open Fable.Core.JsInterop

let voteArea score =
    let icon =
        if score > 0 then
            Fa.ThumbsOUp
        else
            Fa.ThumbsODown

    let iterationCount =
        Math.Min(5, Math.Abs score - 1)

    span [ ]
        [ for i = 0 to iterationCount do
            yield Icon.faIcon [ ] icon ]

let root model dispatch =
    Media.media [ ]
        [ Media.left [ ]
            [ Image.image [ Image.is64x64 ]
                [ img [ Src ("avatars/" + model.Author.Avatar) ] ] ]
          Media.content [ ]
            [ Render.contentFromMarkdown [ ] model.Answer.Content
              Level.level [ ]
                [ Level.right [ Level.customClass "vote-area" ]
                    [ Button.button [ if model.IsLoading then
                                        yield Button.isLoading
                                      yield Button.isSmall
                                      yield Button.isDanger
                                      yield Button.onClick (fun _ -> dispatch VoteDown) ]
                        [ str "-1" ]
                      Button.button [ if model.IsLoading then
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
                                            model.Author.Firstname
                                            model.Author.Surname
                                            (model.Answer.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"))) ] ] ] ] ] ]
