module Sugar

open Elmish
open Fable.Import.React

let secureUpdate (update: 'b -> 'a -> 'a * Cmd<'b>) (msg: 'b) (optionalModel : 'a option) =
    if optionalModel.IsSome then
        update msg optionalModel.Value
    else
        failwith "Optional model has no value"

let secureView (view: 'a -> ('b -> unit) -> ReactElement) (optionalModel : 'a option) =
    if optionalModel.IsSome then
        view optionalModel.Value
    else
        failwith "Optional model has no value"
