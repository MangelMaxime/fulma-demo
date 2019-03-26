module Lowdb

open System
open Fable.Core

    type [<AllowNullLiteral>] PromiseLike<'T> =
        abstract ``then``: ?onfulfilled: Func<'T, U2<'TResult, PromiseLike<'TResult>>> * ?onrejected: Func<obj, U2<'TResult, PromiseLike<'TResult>>> -> PromiseLike<'TResult>
        abstract ``then``: ?onfulfilled: Func<'T, U2<'TResult, PromiseLike<'TResult>>> * ?onrejected: Func<obj, unit> -> PromiseLike<'TResult>

    and [<AllowNullLiteral>] StringRepresentable =
        abstract toString: unit -> string

    and [<AllowNullLiteral>] List<'T> =
        [<Emit("$0[$1]{{=$2}}")>] abstract Item: index: float -> 'T with get, set
        abstract length: float with get, set

    and [<AllowNullLiteral>] Dictionary<'T> =
        [<Emit("$0[$1]{{=$2}}")>] abstract Item: index: string -> 'T with get, set

    and [<AllowNullLiteral>] DictionaryIterator<'T, 'TResult> =
        [<Emit("$0($1...)")>] abstract Invoke: value: 'T * ?key: string * ?collection: Dictionary<'T> -> 'TResult

    and [<AllowNullLiteral>] ListIterator<'T, 'TResult> =
        [<Emit("$0($1...)")>] abstract Invoke: value: 'T * index: float * collection: List<'T> -> 'TResult

    and [<AllowNullLiteral>] StringIterator<'TResult> =
        [<Emit("$0($1...)")>] abstract Invoke: char: string * ?index: float * ?string: string -> 'TResult

    and [<AllowNullLiteral>] MixinOptions =
        abstract chain: bool option with get, set

    and [<AllowNullLiteral>] [<Import("LoDashWrapper","Lowdb")>] LoDashWrapper<'LowEntryClass>() =
        member __.has(path: U2<StringRepresentable, ResizeArray<StringRepresentable>>): 'LowEntryClass = jsNative
        member __.hasIn(path: U2<StringRepresentable, ResizeArray<StringRepresentable>>): 'LowEntryClass = jsNative
        member __.assign(source: 'TSource): 'LowEntryClass = jsNative
        member __.assign(source1: 'TSource1, source2: 'TSource2): 'LowEntryClass = jsNative
        member __.assign(source1: 'TSource1, source2: 'TSource2, source3: 'TSource3): 'LowEntryClass = jsNative
        member __.assign(source1: 'TSource1, source2: 'TSource2, source3: 'TSource3, source4: 'TSource4): 'LowEntryClass = jsNative
        member __.assign(): 'LowEntryClass = jsNative
        member __.assign([<ParamArray>] otherArgs: obj[]): 'LowEntryClass = jsNative
        member __.cloneDeep(): 'LowEntryClass = jsNative
        member __.cloneDeepWith(customizer: Func<obj, obj>): ResizeArray<'LowEntryClass> = jsNative
        member __.defaults(source1: 'S1, [<ParamArray>] sources: obj[]): 'LowEntryClass = jsNative
        member __.defaults(source1: 'S1, source2: 'S2, [<ParamArray>] sources: obj[]): 'LowEntryClass = jsNative
        member __.defaults(source1: 'S1, source2: 'S2, source3: 'S3, [<ParamArray>] sources: obj[]): 'LowEntryClass = jsNative
        member __.defaults(source1: 'S1, source2: 'S2, source3: 'S3, source4: 'S4, [<ParamArray>] sources: obj[]): 'LowEntryClass = jsNative
        member __.defaults(): 'LowEntryClass = jsNative
        member __.defaults([<ParamArray>] sources: obj[]): 'LowEntryClass = jsNative
        member __.get(``object``: obj, path: U4<string, float, bool, ResizeArray<U3<string, float, bool>>>, ?defaultValue: 'TResult): 'LowEntryClass = jsNative
        member __.get(path: U4<string, float, bool, ResizeArray<U3<string, float, bool>>>, ?defaultValue: 'TResult): 'LowEntryClass = jsNative
        member __.``mixin``(source: Dictionary<Func<unit, unit>>, ?options: MixinOptions): 'LowEntryClass = jsNative
        member __.``mixin``(?options: MixinOptions): 'LowEntryClass = jsNative
        member __.set(path: U2<StringRepresentable, ResizeArray<StringRepresentable>>, value: obj): 'LowEntryClass = jsNative
        member __.set(path: U2<StringRepresentable, ResizeArray<StringRepresentable>>, value: 'V): 'LowEntryClass = jsNative
        member __.find(?predicate: ListIterator<'T, bool>, ?thisArg: obj): 'LowEntryClass = jsNative
        member __.find(?predicate: string, ?thisArg: obj): 'LowEntryClass = jsNative
        member __.find(?predicate: 'TObject): 'LowEntryClass = jsNative
        member __.filter(?predicate: 'TObject): 'LowEntryClass = jsNative
        // member __.filter(?predicate: ListIterator<'T, bool>, ?thisArg: obj): 'LowEntryClass = jsNative
        // member __.filter(predicate: string, ?thisArg: obj): 'LowEntryClass = jsNative
        // member __.filter(predicate: U2<ListIterator<'T, bool>, DictionaryIterator<'T, bool>>, ?thisArg: obj): 'LowEntryClass = jsNative
        // member __.filter(?predicate: StringIterator<bool>, ?thisArg: obj): 'LowEntryClass = jsNative
        // member __.filter(predicate: 'W): 'LowEntryClass = jsNative
        member __.map(?iteratee: ListIterator<'T, 'TResult>, ?thisArg: obj): 'LowEntryClass = jsNative
        member __.map(?iteratee: string): 'LowEntryClass = jsNative
        member __.map(?iteratee: 'TObject): 'LowEntryClass = jsNative
        member __.map(?iteratee: U2<ListIterator<'TValue, 'TResult>, DictionaryIterator<'TValue, 'TResult>>, ?thisArg: obj): 'LowEntryClass = jsNative
        member __.range(?``end``: float, ?step: float): 'LowEntryClass = jsNative
        member __.rangeRight(?``end``: float, ?step: float): 'LowEntryClass = jsNative
        member __.remove(?predicate: ListIterator<'T, bool>, ?thisArg: obj): 'LowEntryClass = jsNative
        member __.remove(?predicate: string, ?thisArg: obj): 'LowEntryClass = jsNative
        member __.remove(?predicate: 'W): 'LowEntryClass = jsNative
        member __.sortBy(?iteratee: ListIterator<'T, 'TSort>): 'LowEntryClass = jsNative
        member __.sortBy(iteratee: string): 'LowEntryClass = jsNative
        member __.sortBy(whereValue: 'W): 'LowEntryClass = jsNative
        member __.sortBy(): 'LowEntryClass = jsNative
        member __.sortBy([<ParamArray>] iteratees: U3<ListIterator<'T, bool>, obj, string>[]): 'LowEntryClass = jsNative
        member __.sortBy(iteratees: ResizeArray<U3<ListIterator<'T, obj>, string, obj>>): 'LowEntryClass = jsNative
        member __.slice(?start: float, ?``end``: float): 'LowEntryClass = jsNative
        member __.size(): 'LowEntryClass = jsNative
        member __.take(?n: float): 'LowEntryClass = jsNative
        member __.times(iteratee: Func<float, 'TResult>): 'LowEntryClass = jsNative
        member __.times(): 'LowEntryClass = jsNative
        member __.uniqueId(): 'LowEntryClass = jsNative
        member __.value(): 'T = jsNative
        member __.pop(): 'T = jsNative
        member __.push([<ParamArray>] items: 'T[]): 'LowEntryClass = jsNative
        member __.shift(): 'T = jsNative
        member __.sort(?compareFn: Func<'T, 'T, float>): 'LowEntryClass = jsNative
        member __.splice(start: float): 'LowEntryClass = jsNative
        member __.splice(start: float, deleteCount: float, [<ParamArray>] items: obj[]): 'LowEntryClass = jsNative
        member __.unshift([<ParamArray>] items: 'T[]): 'LowEntryClass = jsNative

    and [<AllowNullLiteral>] Storage =
        abstract read: source: string * deserialize: obj -> PromiseLike<obj>
        abstract write: destination: string * obj: obj * serialize: obj -> unit

    and [<AllowNullLiteral>] Format =
        abstract serialize: obj: obj -> string
        abstract deserialize: data: string -> obj

    and [<AllowNullLiteral>] Options =
        abstract storage: Storage option with get, set
        abstract format: Format option with get, set
        abstract writeOnChange: bool option with get, set

    type [<AllowNullLiteral>] AdapterOptions =
        abstract defaultValue : 'T option with get, set
        abstract serialize : ('T -> string) option with get, set
        abstract deserialize : (string -> 'T) option with get, set

    and IAdapter(source: string, ?option: AdapterOptions) =
        class end

    and [<Import("*", "lowdb/adapters/FileAsync")>] FileAsyncAdapter(source: string, ?option: AdapterOptions) =
        inherit IAdapter(source, ?option = option)

    and [<Import("*", "lowdb/adapters/FileSync")>] FileSyncAdapter(source: string, ?option: AdapterOptions) =
        inherit IAdapter(source, ?option = option)

    and [<Import("*", "lowdb/adapters/Memory")>] MemoryAdapter(source: string, ?option: AdapterOptions) =
        inherit IAdapter(source, ?option = option)

    and [<Import("*", "lowdb/adapters/LocalStorage")>] LocalStorageAdapter(source: string, ?option: AdapterOptions) =
        inherit IAdapter(source, ?option = option)

    // type Adapter =
    //      static member FileAsync(source: string, ?options: AdapterOptions) :FileAsyncAdapter = jsNative

    // let FileAsyncAdapter =

    type [<AllowNullLiteral>] [<Import("*","lowdb")>] Lowdb(adapter: IAdapter, ?options: Options) =
        inherit LoDashWrapper<Lowdb>()
        member __.getState(): obj = jsNative
        member __.setState(newState: obj): unit = jsNative
        member __.write(?source: string): unit = jsNative
        member __.read(?source: string): obj = jsNative
