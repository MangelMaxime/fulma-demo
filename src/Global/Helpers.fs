module Cmd

    open Elmish
    open System

    let mapWithGuid
            (outMsg : Guid * 'subCmd -> 'msg)
            (guid : Guid)
            (cmd : Cmd<'subCmd>) : Cmd<'msg> =
        Cmd.map (fun msg -> outMsg (guid, msg)) cmd
