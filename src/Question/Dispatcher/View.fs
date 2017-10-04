module Question.Dispatcher.View

open Types
open Fable.Helpers.React
open Fable.Helpers.React.Props

let root model dispatch =
    match model.CurrentPage with
    | Router.QuestionPage.Index -> Question.Index.View.root model.IndexModel (IndexMsg >> dispatch)
    | Router.QuestionPage.Show id -> Question.Show.View.root model.ShowModel (ShowMsg >> dispatch)
