module Logger

    open Fable.Import.JS

    let inline error format = Printf.ksprintf (fun x -> console.error x) format

    let inline log format = Printf.ksprintf (fun x -> console.log x) format

    let inline warn format = Printf.ksprintf (fun x -> console.warn x) format
