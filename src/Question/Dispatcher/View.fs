module Question.Dispatcher.View

open Types
open Fable.Helpers.React
open Fable.Helpers.React.Props

let root model (currentPage : Navigation.QuestionPage) dispatch =
    match currentPage with
    | Navigation.QuestionPage.Index -> Question.Index.View.root model.IndexModel (IndexMsg >> dispatch)
