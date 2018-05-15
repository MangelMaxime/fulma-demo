module LibB

open Fable.Core

type Exports =
    abstract ShowInConsole: msg : string -> unit

let showInConsole (msg : string) = Fable.Import.Browser.console.log(msg)

[<ExportDefault>]
let exports =
    { new Exports with
            member __.ShowInConsole(msg) = showInConsole msg }
