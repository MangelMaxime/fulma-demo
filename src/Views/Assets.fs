module Views.Assets

open Fulma.Elements
open Fable.Helpers.React
open Fable.Helpers.React.Props

let avatar64x64 url =
    Image.image [ Image.is64x64 ]
        [ img [ Src ("avatars/" + url)  ] ]
