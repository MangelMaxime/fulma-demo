module ThemeLoader

open Fable.Core
open Fable.React
open Browser.CssExtensions

type ThemeLoaderProps =
    { Theme : string }


type ThemeLoader(initProps) =
    inherit Component<ThemeLoaderProps, obj>(initProps)

    let classIdentifier = "theme-loader"

    member this.updateStyle() =
        let styles = Browser.Dom.document.styleSheets
        for index = 0 to (int styles.length) - 1 do
            let currentStyle = styles.item (float index)

            currentStyle.disabled <- currentStyle.href.Contains("." + this.props.Theme + ".")

    override this.componentDidMount() =
        this.updateStyle()

    override this.componentWillReceiveProps(nextProps) =
        if nextProps.Theme <> this.props.Theme then
            this.updateStyle()

    override this.render() =
        nothing
