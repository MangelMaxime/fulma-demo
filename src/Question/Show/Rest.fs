module Question.Show.Rest

open Fable.PowerPack
open Types
open Database

let getDetails (id : int) =
    promise {

        let questions =
            Database.Questions
                .sortBy("Id")
                .value()
            |> Array.toList

        do! Promise.sleep 1000

        return GetDetailsRes.Success (failwith "")
    }
