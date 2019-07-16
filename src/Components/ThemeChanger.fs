module ThemeChanger

open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Browser

let hot = HMR.``module``.hot

[<Emit("new MutationObserver($0)")>]
let newMutationObserver (callback : obj list -> obj -> unit) : obj = jsNative

type ThemeChangerProps =
    {
        Theme : string
    }

type ThemeChanger(initProps) =
    inherit Component<ThemeChangerProps, obj>(initProps)

    let mutable observer = null

    member __.removeObserver() =
        if not (isNull observer) then
            observer?disconnect()

    member this.registerObserver() =
        let targetNode = document.querySelector("body")

        let config =
            createObj [
                "childList" ==> true
            ]

        let callback (mutationList : obj list) (_observer : obj) =
            for mutation in mutationList do
                if mutation?``type`` = "childList" then
                    let nodes =
                        [
                            for node in mutation?addedNodes do
                                yield node :> Types.Node
                            for node in mutation?removedNodes do
                                yield node :> Types.Node
                        ]

                    let shouldUpdateStyle =
                        nodes
                        |> List.exists (fun node ->
                            node.nodeName = "LINK"
                        )

                    if shouldUpdateStyle then
                        this.updateStyle(this.props.Theme)

        observer <- newMutationObserver(callback)

        observer?observe(targetNode, config)

    member __.updateStyle(activeTheme : string) =
        let styles = document.styleSheets
        for index = 0 to (int styles.length) - 1 do
            let currentStyle = styles.item (float index)

            currentStyle.disabled <- not <| currentStyle.href.Contains("." + activeTheme + ".")

    override this.componentDidMount() =
        this.updateStyle(this.props.Theme)

        // Register the observer only if HMR is detected
        if not (isNull hot) then
            this.registerObserver()

    override this.componentWillUnmount() =
        this.removeObserver()

    override this.componentWillReceiveProps(nextProps) =
        if nextProps.Theme <> this.props.Theme then
            this.updateStyle(nextProps.Theme)

    override __.render() =
        nothing
