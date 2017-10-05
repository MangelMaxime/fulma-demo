module Okular

module Lens =
    type Lens<'a,'b> =
        { Get: 'a -> 'b
          Set: 'b -> 'a -> 'a }

        member l.Update outer inner =
            let value = l.Get inner
            let newValue = outer value
            l.Set newValue inner

    let set (l1: Lens<_,_>) value =
        l1.Set value

    let compose (l1: Lens<_,_>) (l2: Lens<_,_>) =
        { Get = l1.Get >> l2.Get
          Set = l2.Set >> l1.Update }

module Operators =
    let inline (>->) (l1: Lens.Lens<_,_>) (l2: Lens.Lens<_,_>) = Lens.compose l1 l2
