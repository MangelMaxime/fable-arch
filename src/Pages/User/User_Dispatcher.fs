namespace WebApp.Pages.User

open Fable.Core
open Fable.Import.Browser

open Fable.Arch
open Fable.Arch.App
open Fable.Arch.Html

open WebApp.Common

module Dispatcher =

  type Model =
    { Index: Index.Model option
      Create: Create.Model option
      Edit: Edit.Model option
      Show: Show.Model option
    }

    static member Generate (?index, ?show, ?edit, ?create) =
      { Index = index
        Create = create
        Edit = edit
        Show =  show
      }

    static member Initial (currentPage: UserApi.Route) =
      match currentPage with
      | UserApi.Index -> Model.Generate (index = Index.Model.Initial)
      | UserApi.Create -> Model.Generate (create = Create.Model.Initial)
      | UserApi.Edit id -> Model.Generate (edit = Edit.Model.Initial)
      | UserApi.Show id -> Model.Generate (show = Show.Model.Initial)


  type Actions
    = IndexActions of Index.Actions
    | ShowActions of Show.Actions
    | EditActions of Edit.Actions
    | CreateActions of Create.Actions

  let update model action =
    match action with
    | IndexActions act ->
        let (res, action) = Index.update model.Index.Value act
        let action' = mapActions IndexActions action
        { model with Index = Some res}, action'
    | CreateActions act ->
        let (res, action) = Create.update model.Create.Value act
        let action' = mapActions IndexActions action
        { model with Create = Some res}, action'
    | EditActions act ->
        let (res, action) = Edit.update model.Edit.Value act
        let action' = mapActions EditActions action
        { model with Edit = Some res}, action'
    | ShowActions act ->
        let (res, action) = Show.update model.Show.Value act
        let action' = mapActions ShowActions action
        { model with Show = Some res}, action'


  let view model subRoute =
    match subRoute with
    | UserApi.Index ->
        Html.map IndexActions (Index.view model.Index.Value)
    | UserApi.Create ->
        Html.map CreateActions (Create.view model.Create.Value)
    | UserApi.Edit _ ->
        Html.map EditActions (Edit.view model.Edit.Value)
    | UserApi.Show _ ->
        Html.map ShowActions (Show.view model.Show.Value)


