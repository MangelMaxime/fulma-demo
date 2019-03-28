module Question.Create.View

open Types
open Fable.React
open Fable.React.Props
open Fulma
open Fable.Core.JsInterop


let private form (user : Database.User) (model : Model) dispatch =
    Media.media [ ]
        [ Media.left [ ]
            [ Image.image [ Image.Is64x64 ]
                [ img [ Src ("avatars/" + user.Avatar) ] ] ]
          Media.content [ ]
            [ form [ ]
                [ Field.div [ ]
                    [ Label.label [ ]
                        [ str "Title" ]
                      Control.div [ Control.IsLoading model.IsWaitingServer ]
                        [ Input.text [ Input.Placeholder "Ex: Who created F#?"
                                       Input.Value model.Title
                                       Input.Disabled model.IsWaitingServer
                                       Input.Props [ OnChange (fun ev -> !!ev.target?value |> ChangeTitle |> dispatch ) ] ] ]
                      Help.help [ Help.Color IsDanger ]
                        [ str model.TitleError ] ]
                  Field.div [ ]
                    [ Label.label [ ]
                        [ str "Description" ]
                      Control.div [ Control.IsLoading model.IsWaitingServer ]
                        [ Textarea.textarea [ Textarea.Value model.Content
                                              Textarea.Disabled model.IsWaitingServer
                                              Textarea.Props [ OnChange (fun ev -> !!ev.target?value |> ChangeContent |> dispatch ) ] ]
                            [ ] ]
                      Help.help [ Help.Color IsDanger ]
                        [ str model.ContentError ] ] ]
              br [ ]
              Level.level [ ]
                [ Level.left [ ]
                    [ Level.item [ ]
                        [ Button.button [ Button.Color IsPrimary
                                          Button.OnClick (fun _ -> dispatch Submit)
                                          Button.Disabled model.IsWaitingServer
                                          ]
                                        [ str "Submit" ] ] ]
                  Level.item [ Level.Item.HasTextCentered ]
                    [ Help.help [ ]
                        [ str "You can use markdown to format your description" ] ] ] ] ]

let root user model dispatch =
    Container.container [ ]
        [ Section.section [ ]
            [ Heading.h3 [ ]
                [ str "Ask a new question" ] ]
          form user model dispatch
        ]
