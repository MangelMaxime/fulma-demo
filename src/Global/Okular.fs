module Okular

let inline flip f a b = f b a

module Lens =
    type Lens<'a,'b> =
        { Get: 'a -> 'b
          Set: 'b -> 'a -> 'a }

    let compose (outer: Lens<_,_>) (inner: Lens<_,_>) =
        let set c a =
            outer.Get a
            |> inner.Set c
            |> (fun b -> outer.Set b a)

        { Get = outer.Get >> inner.Get
          Set = set }

module Prism =
    type Prism<'a,'b> =
        { GetOption : 'a -> 'b option
          ReverseGet : 'b -> 'a }

    let isMatching (prism: Prism<_,_>) value =
        match prism.GetOption value with
        | Some v -> true
        | None -> false


    ///**Description**
    /// Modifies given function `('b -> 'b)` to be `('a -> Some 'a)` using `Prism<'a, 'b>`
    /// Code:
    /// ```
    /// fx i = i * 2
    /// modified = Monocle.Prism.modify string2IntPrism fx
    /// modified "22" == Just "44"
    /// modified "abc" == Nothing
    /// ```
    ///**Parameters**
    ///  * `prism` - parameter of type `Prism<'a,'b>`
    ///  * `f` - parameter of type `'b -> 'b`
    ///
    ///**Output Type**
    ///  * `'a -> 'a option`
    ///
    let modifyOption (prism: Prism<_,_>) f =
        prism.GetOption >> Option.map (f >> prism.ReverseGet)


    ///**Description**
    /// Modifies given function `('b -> 'b)` to be `('a -> 'a)` using `Prism<'a,'b>`
    /// Code:
    /// ```
    /// fx i = i * 2
    /// modified = Monocle.Prism.modify string2IntPrism fx
    /// modified "22" == "44"
    /// modified "abc" == "abc"
    /// ```
    ///**Parameters**
    ///  * `prism` - parameter of type `Prism<'a,'b>`
    ///  * `f` - parameter of type `'b -> 'b`
    ///  * `x` - parameter of type `'a`
    ///
    ///**Output Type**
    ///  * `'a`
    ///
    let modify (prism: Prism<_,_>) f =
        (fun x -> modifyOption prism f x |> Option.defaultValue x)


    let compose (outer: Prism<_,_>) (inner: Prism<_,_>) =
        let getOption x =
            match outer.GetOption x with
            | Some y ->
                y |> inner.GetOption

            | None ->
                None

        { GetOption = getOption
          ReverseGet = inner.ReverseGet >> outer.ReverseGet }

module Optional =
    type Optional<'a,'b> =
        { GetOption : 'a -> 'b option
          Set : 'b -> 'a -> 'a }

    let compose outer inner  =
        let set c a =
            outer.GetOption a
            |> Option.map (inner.Set c >> flip outer.Set a)
            |> Option.defaultValue a

        let getOption a =
            match outer.GetOption a with
            | Some x ->
                x |> inner.GetOption
            | None -> None

        { GetOption = getOption
          Set = set }

    let composeLens (opt: Optional<_,_>) (lens: Lens.Lens<_,_>) =
        let set c a =
            opt.GetOption a
            |> Option.map (lens.Set c >> flip opt.Set a)
            |> Option.defaultValue a

        let getOption a =
            match opt.GetOption a with
            | Some b ->
                b |> lens.Get |> Some
            | None ->
                None

        { GetOption = getOption
          Set = set }

    let modifyOption (opt: Optional<'a,'b>) (fx: 'b -> 'b) =
        (fun a -> opt.GetOption a |> Option.map (fx >> flip opt.Set a))

    let modify (opt: Optional<'a,'b>) (fx: 'b -> 'b) =
        (fun a -> modifyOption opt fx a |> Option.defaultValue a)

module Operators =
    let inline (>->) (l1: Lens.Lens<_,_>) (l2: Lens.Lens<_,_>) = Lens.compose l1 l2

module Sugar =

    let Lens a b : Lens.Lens<_,_> =
        { Get = a
          Set = b }

    let Prism a b : Prism.Prism<_,_> =
        { GetOption = a
          ReverseGet = b }

    let Optional a b : Optional.Optional<_,_> =
        { GetOption = a
          Set = b }
