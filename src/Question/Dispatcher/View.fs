module Question.Dispatcher.View

open Fable.React
open Types

let QuestionShow =
    FunctionComponent.Lazy(
        Question.Show.View.root,
        fallback = div [] [str "Loading..."])

let root user model dispatch =
    match model with
    | { CurrentPage = Router.QuestionPage.Index
        IndexModel = Some extractedModel } -> Question.Index.View.root extractedModel (IndexMsg >> dispatch)

    | { CurrentPage = Router.QuestionPage.Show _
        ShowModel = Some extractedModel } ->
            QuestionShow {| user = user
                            model = extractedModel
                            dispatch = ShowMsg >> dispatch |}

    | { CurrentPage = Router.QuestionPage.Create
        CreateModel = Some extractedModel } -> Question.Create.View.root user extractedModel (CreateMsg >> dispatch)

    | _ ->
        Render.pageNotFound
