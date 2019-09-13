// ts2fable 0.6.2
module rec FakerJS
open System
open Fable.Core
open System.Text.RegularExpressions

let [<Import("*","faker")>] fakerStatic: Faker.FakerStatic = jsNative

module Faker =

    type [<AllowNullLiteral>] FakerStatic =
        abstract locale: string with get, set
        abstract setLocale: locale: string -> unit
        abstract address: TypeLiteral_01 with get, set
        abstract commerce: TypeLiteral_02 with get, set
        abstract company: TypeLiteral_03 with get, set
        abstract database: TypeLiteral_04 with get, set
        abstract date: TypeLiteral_06 with get, set
        abstract fake: str: string -> string
        abstract finance: TypeLiteral_07 with get, set
        abstract hacker: TypeLiteral_08 with get, set
        abstract helpers: TypeLiteral_10<'T> with get, set
        abstract image: TypeLiteral_11 with get, set
        abstract internet: TypeLiteral_12 with get, set
        abstract lorem: TypeLiteral_13 with get, set
        abstract name: TypeLiteral_14 with get, set
        abstract phone: TypeLiteral_15 with get, set
        abstract random: TypeLiteral_19<'T> with get, set
        abstract system: TypeLiteral_20 with get, set
        abstract seed: value: float -> unit

    type [<AllowNullLiteral>] Card =
        abstract name: string with get, set
        abstract username: string with get, set
        abstract email: string with get, set
        abstract address: FullAddress with get, set
        abstract phone: string with get, set
        abstract website: string with get, set
        abstract company: Company with get, set
        abstract posts: ResizeArray<Post> with get, set
        abstract accountHistory: ResizeArray<string> with get, set

    type [<AllowNullLiteral>] FullAddress =
        abstract streetA: string with get, set
        abstract streetB: string with get, set
        abstract streetC: string with get, set
        abstract streetD: string with get, set
        abstract city: string with get, set
        abstract state: string with get, set
        abstract county: string with get, set
        abstract zipcode: string with get, set
        abstract geo: Geo with get, set

    type [<AllowNullLiteral>] Geo =
        abstract lat: string with get, set
        abstract lng: string with get, set

    type [<AllowNullLiteral>] Company =
        abstract name: string with get, set
        abstract catchPhrase: string with get, set
        abstract bs: string with get, set

    type [<AllowNullLiteral>] Post =
        abstract words: string with get, set
        abstract sentence: string with get, set
        abstract sentences: string with get, set
        abstract paragraph: string with get, set

    type [<AllowNullLiteral>] ContextualCard =
        abstract name: string with get, set
        abstract username: string with get, set
        abstract email: string with get, set
        abstract dob: DateTime with get, set
        abstract phone: string with get, set
        abstract address: Address with get, set
        abstract website: string with get, set
        abstract company: Company with get, set

    type [<AllowNullLiteral>] Address =
        abstract street: string with get, set
        abstract suite: string with get, set
        abstract city: string with get, set
        abstract state: string with get, set
        abstract zipcode: string with get, set
        abstract geo: Geo with get, set

    type [<AllowNullLiteral>] UserCard =
        abstract name: string with get, set
        abstract username: string with get, set
        abstract email: string with get, set
        abstract address: Address with get, set
        abstract phone: string with get, set
        abstract website: string with get, set
        abstract company: Company with get, set

    type [<AllowNullLiteral>] Transaction =
        abstract amount: string with get, set
        abstract date: DateTime with get, set
        abstract business: string with get, set
        abstract name: string with get, set
        abstract ``type``: string with get, set
        abstract account: string with get, set

    type [<AllowNullLiteral>] TypeLiteral_05 =
        abstract abbr: bool option with get, set
        abstract context: bool option with get, set

    type [<AllowNullLiteral>] TypeLiteral_16 =
        abstract min: float option with get, set
        abstract max: float option with get, set
        abstract precision: float option with get, set

    type [<AllowNullLiteral>] TypeLiteral_18<'T> =
        [<Emit "$0[$1]{{=$2}}">] abstract Item: key: string -> 'T with get, set

    type [<AllowNullLiteral>] TypeLiteral_09 =
        [<Emit "$0[$1]{{=$2}}">] abstract Item: key: string -> U2<string, (string -> ResizeArray<obj option> -> string)> with get, set

    type [<AllowNullLiteral>] TypeLiteral_17 =
        [<Emit "$0[$1]{{=$2}}">] abstract Item: key: string -> obj option with get, set

    type [<AllowNullLiteral>] TypeLiteral_08 =
        abstract abbreviation: unit -> string
        abstract adjective: unit -> string
        abstract noun: unit -> string
        abstract verb: unit -> string
        abstract ingverb: unit -> string
        abstract phrase: unit -> string

    type [<AllowNullLiteral>] TypeLiteral_07 =
        abstract account: ?length: float -> string
        abstract accountName: unit -> string
        abstract mask: ?length: float * ?parens: bool * ?elipsis: bool -> string
        abstract amount: ?min: float * ?max: float * ?dec: float * ?symbol: string -> string
        abstract transactionType: unit -> string
        abstract currencyCode: unit -> string
        abstract currencyName: unit -> string
        abstract currencySymbol: unit -> string
        abstract bitcoinAddress: unit -> string
        abstract ethereumAddress: unit -> string
        abstract iban: ?formatted: bool -> string
        abstract bic: unit -> string

    type [<AllowNullLiteral>] TypeLiteral_12 =
        abstract avatar: unit -> string
        abstract email: ?firstName: string * ?lastName: string * ?provider: string -> string
        abstract exampleEmail: ?firstName: string * ?lastName: string -> string
        abstract userName: ?firstName: string * ?lastName: string -> string
        abstract protocol: unit -> string
        abstract url: unit -> string
        abstract domainName: unit -> string
        abstract domainSuffix: unit -> string
        abstract domainWord: unit -> string
        abstract ip: unit -> string
        abstract ipv6: unit -> string
        abstract userAgent: unit -> string
        abstract color: ?baseRed255: float * ?baseGreen255: float * ?baseBlue255: float -> string
        abstract mac: unit -> string
        abstract password: ?len: float * ?memorable: bool * ?pattern: U2<string, Regex> * ?prefix: string -> string

    type [<AllowNullLiteral>] TypeLiteral_02 =
        abstract color: unit -> string
        abstract department: unit -> string
        abstract productName: unit -> string
        abstract price: ?min: float * ?max: float * ?dec: float * ?symbol: string -> string
        abstract productAdjective: unit -> string
        abstract productMaterial: unit -> string
        abstract product: unit -> string

    type [<AllowNullLiteral>] TypeLiteral_04 =
        abstract column: unit -> string
        abstract ``type``: unit -> string
        abstract collation: unit -> string
        abstract engine: unit -> string

    type [<AllowNullLiteral>] TypeLiteral_20 =
        abstract fileName: ?ext: string * ?``type``: string -> string
        abstract commonFileName: ext: string * ?``type``: string -> string
        abstract mimeType: unit -> string
        abstract commonFileType: unit -> string
        abstract commonFileExt: unit -> string
        abstract fileType: unit -> string
        abstract fileExt: mimeType: string -> string
        abstract directoryPath: unit -> string
        abstract filePath: unit -> string
        abstract semver: unit -> string

    type [<AllowNullLiteral>] TypeLiteral_14 =
        abstract firstName: ?gender: float -> string
        abstract lastName: ?gender: float -> string
        abstract findName: ?firstName: string * ?lastName: string * ?gender: float -> string
        abstract jobTitle: unit -> string
        abstract prefix: unit -> string
        abstract suffix: unit -> string
        abstract title: unit -> string
        abstract jobDescriptor: unit -> string
        abstract jobArea: unit -> string
        abstract jobType: unit -> string

    type [<AllowNullLiteral>] TypeLiteral_11 =
        abstract image: unit -> string
        abstract avatar: unit -> string
        abstract imageUrl: ?width: float * ?height: float * ?category: string * ?randomize: bool * ?https: bool -> string
        abstract ``abstract``: ?width: float * ?height: float -> string
        abstract animals: ?width: float * ?height: float -> string
        abstract business: ?width: float * ?height: float -> string
        abstract cats: ?width: float * ?height: float -> string
        abstract city: ?width: float * ?height: float -> string
        abstract food: ?width: float * ?height: float -> string
        abstract nightlife: ?width: float * ?height: float -> string
        abstract fashion: ?width: float * ?height: float -> string
        abstract people: ?width: float * ?height: float -> string
        abstract nature: ?width: float * ?height: float -> string
        abstract sports: ?width: float * ?height: float -> string
        abstract technics: ?width: float * ?height: float -> string
        abstract transport: ?width: float * ?height: float -> string
        abstract dataUri: ?width: float * ?height: float -> string

    type [<AllowNullLiteral>] TypeLiteral_19<'T> =
        abstract number: max: float -> float
        abstract number: ?options: TypeLiteral_16 -> float
        abstract arrayElement: unit -> string
        abstract arrayElement: array: ResizeArray<'T> -> 'T
        [<Emit "$0.objectElement($1,'key')">] abstract objectElement_key: ?``object``: TypeLiteral_17 -> string
        abstract objectElement: ?``object``: TypeLiteral_18<'T> * ?field: obj -> 'T
        abstract uuid: unit -> string
        abstract boolean: unit -> bool
        abstract word: ?``type``: string -> string
        abstract words: ?count: float -> string
        abstract image: unit -> string
        abstract locale: unit -> string
        abstract alphaNumeric: ?count: float -> string
        abstract hexaDecimal: ?count: float -> string

    type [<AllowNullLiteral>] TypeLiteral_06 =
        abstract past: ?years: float * ?refDate: U2<string, DateTime> -> DateTime
        abstract future: ?years: float * ?refDate: U2<string, DateTime> -> DateTime
        abstract between: from: U3<string, float, DateTime> * ``to``: U2<string, DateTime> -> DateTime
        abstract recent: ?days: float -> DateTime
        abstract soon: ?days: float -> DateTime
        abstract month: ?options: TypeLiteral_05 -> string
        abstract weekday: ?options: TypeLiteral_05 -> string

    type [<AllowNullLiteral>] TypeLiteral_15 =
        abstract phoneNumber: ?format: string -> string
        abstract phoneNumberFormat: ?phoneFormatsArrayIndex: float -> string
        abstract phoneFormats: unit -> string

    type [<AllowNullLiteral>] TypeLiteral_10<'T> =
        abstract randomize: array: ResizeArray<'T> -> 'T
        abstract randomize: unit -> string
        abstract slugify: ?string: string -> string
        abstract replaceSymbolWithNumber: ?string: string * ?symbol: string -> string
        abstract replaceSymbols: ?string: string -> string
        abstract shuffle: o: ResizeArray<'T> -> ResizeArray<'T>
        abstract shuffle: unit -> ResizeArray<string>
        abstract mustache: str: string * data: TypeLiteral_09 -> string
        abstract createCard: unit -> Card
        abstract contextualCard: unit -> ContextualCard
        abstract userCard: unit -> UserCard
        abstract createTransaction: unit -> Transaction

    type [<AllowNullLiteral>] TypeLiteral_03 =
        abstract suffixes: unit -> ResizeArray<string>
        abstract companyName: ?format: float -> string
        abstract companySuffix: unit -> string
        abstract catchPhrase: unit -> string
        abstract bs: unit -> string
        abstract catchPhraseAdjective: unit -> string
        abstract catchPhraseDescriptor: unit -> string
        abstract catchPhraseNoun: unit -> string
        abstract bsAdjective: unit -> string
        abstract bsBuzz: unit -> string
        abstract bsNoun: unit -> string

    type [<AllowNullLiteral>] TypeLiteral_13 =
        abstract word: unit -> string
        abstract words: ?num: float -> string
        abstract sentence: ?wordCount: float * ?range: float -> string
        abstract slug: ?wordCount: float -> string
        abstract sentences: ?sentenceCount: float -> string
        abstract paragraph: ?sentenceCount: float -> string
        abstract paragraphs: ?paragraphCount: float * ?separator: string -> string
        abstract text: ?times: float -> string
        abstract lines: ?lineCount: float -> string

    type [<AllowNullLiteral>] TypeLiteral_01 =
        abstract zipCode: ?format: string -> string
        abstract city: ?format: float -> string
        abstract cityPrefix: unit -> string
        abstract citySuffix: unit -> string
        abstract streetName: unit -> string
        abstract streetAddress: ?useFullAddress: bool -> string
        abstract streetSuffix: unit -> string
        abstract streetPrefix: unit -> string
        abstract secondaryAddress: unit -> string
        abstract county: unit -> string
        abstract country: unit -> string
        abstract countryCode: unit -> string
        abstract state: ?useAbbr: bool -> string
        abstract stateAbbr: unit -> string
        abstract latitude: unit -> string
        abstract longitude: unit -> string
