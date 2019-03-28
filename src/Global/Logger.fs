module Logger

    open Fable.Core.JS

    let inline error msg = console.error msg

    let inline errorfn fn msg = printfn fn msg

    let inline log msg = console.log msg

    let inline debug (info: obj) = console.log("[Debug]", info)

    let inline debugfn fn info = console.log("[Debug] " + sprintf fn info)
