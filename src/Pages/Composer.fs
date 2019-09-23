module Mailbox.Composer

open Fulma
open Elmish
open Fable.FontAwesome
open Fable.React
open Fable.React.Props
open System
open Helpers

type Model =
    {
        Composers : Map<Guid, Composer.Editor.Model>
    }

type Msg =
    | ComposerMsg of Guid * Composer.Editor.Msg
    | ComposeNewEmail
    | CloseComposer of Guid

let init () =
    {
        Composers = Map.empty
    }

let update (msg  : Msg) (model : Model) =
    match msg with
    | ComposerMsg (composerId, composerMsg) ->
        match Map.tryFind composerId model.Composers with
        | Some composerModel ->
            let (composerModel, composerMsg) = Composer.Editor.update composerId composerMsg composerModel
            { model with
                Composers = Map.add composerId composerModel model.Composers
            }
            , Cmd.mapWithIdentifier ComposerMsg composerId composerMsg

        | None ->
            model, Cmd.none

    | ComposeNewEmail ->
        let composers = Map.add (Guid.NewGuid()) (Composer.Editor.init None) model.Composers
        { model with
            Composers = composers
        }
        , Cmd.none

    | CloseComposer composerId ->
        { model with
            Composers = Map.remove composerId model.Composers
        }
        , Cmd.none

let composeButton (dispatch : Dispatch<Msg>) =
    Button.button
        [
            Button.Color IsPrimary
            Button.IsFullWidth
            Button.Modifiers [ Modifier.TextWeight TextWeight.Bold ]
            Button.OnClick (fun _ ->
                dispatch ComposeNewEmail

            )
        ]
        [ str "Compose" ]

type EventListenerProps =
    {
        Dispatch : Dispatch<Msg>
    }

type private EventListener(initProps) =
    inherit Component<EventListenerProps, obj>(initProps)

    let mutable closeEmailHandler = Unchecked.defaultof<Browser.Types.Event -> unit>

    override this.shouldComponentUpdate(nextProps, _) =
        HMR.equalsButFunctions this.props nextProps
        |> not

    override this.componentDidMount() =
        closeEmailHandler <-
            fun (ev : Browser.Types.Event) ->
                let ev = ev :?>  Browser.Types.CustomEvent
                let caller = ev.detail |> unbox<Guid>

                this.props.Dispatch (CloseComposer caller)
                ()

        Browser.Dom.window.addEventListener("composer-editor-ask-to-close", closeEmailHandler)

    override this.componentWillUnmount() =
        Browser.Dom.window.removeEventListener("composer-editor-ask-to-close", closeEmailHandler)

    override this.render() =
        nothing


let view (model : Model) (dispatch : Dispatch<Msg>) =
    div [ Class "composer-container" ]
        [
            ofType<EventListener,_,_> { Dispatch = dispatch } [ ]
            model.Composers
            |> Map.toList
            |> List.sortBy (fun (composerId, composerModel) ->
                composerModel.SortableKey
            )
            |> List.mapi (fun index (composerId, composerModel) ->
                let onClose () =
                    dispatch (CloseComposer composerId)

                let dispatch =
                    (fun msg -> dispatch (ComposerMsg (composerId, msg)) )

                fragment [ FragmentProp.Key ("composer-editor-" + string composerId) ]
                    [
                        Composer.Editor.view index composerModel dispatch onClose
                    ]
            )
            |> ofList
        ]
