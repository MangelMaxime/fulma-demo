module Views.Assets

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma

let avatar64x64 url =
    Image.image [ Image.Is64x64 ]
        [ img [ Src ("avatars/" + url)  ] ]
