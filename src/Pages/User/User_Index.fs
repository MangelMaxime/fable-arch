namespace WebApp.Pages.User

open Fable.Core
open Fable.Import.Browser

open Fable.Arch
open Fable.Arch.App
open Fable.Arch.Html

open WebApp.Common
open WebApp

module Index =

  type Model =
    { Users: UserRecord list
      IsLoading: bool
    }

    static member Initial =
      { Users = []
        IsLoading = true
      }

  type Actions =
    | NoOp
    | Init
    | SetUsers of UserRecord list
    | StopLoading
    | NavigateTo of Route

  let update model action =
    match action with
    | NoOp ->
        model, []
    | Init ->
        let message =
          [ fun h ->
              FakeApi.Get<UserRecord list>
                (FakeApi.Ressources.User FakeApi.UserRes.Index)
                (fun data ->
                  h (SetUsers data)
                  h (StopLoading)
                )
          ]
        model, message
    | SetUsers users ->
        { model with Users = users }, []
    | StopLoading ->
        { model with IsLoading = false}, []
    | NavigateTo route ->
      let message =
        [ fun h ->
            let url = resolveRoutesToUrl route
            match url with
            | Some u -> location.hash <- u
            | None -> failwith "Cannot be reached. Route should always be resolve"
        ]
      model, message

  let header : DomNode<Actions> =
    thead
      []
      [ tr
          []
          [ th
              []
              [ text "Gender" ]
            th
              []
              [ text "Firstname" ]
            th
              []
              [ text "Surname" ]
            th
              []
              [ text "Email" ]
            th
              [ attribute "colspan" "2" ]
              []
          ]
      ]

  let row (item: UserRecord) =
    tr
      [ classy "is-clickable"
        onMouseClick(fun _ ->
          console.log "row"; NoOp
        )
      ]
      [ td
          []
          [text (item.Gender.ToString()) ]
        td
          []
          [ text item.Firstname ]
        td
          []
          [ text item.Surname ]
        td
          []
          [ text item.Email ]
        td
          [ classy "is-icon" ]
          [ a
              [ voidLinkAction<Actions>
                onMouseClick(fun _ ->
                  console.log "icon"; NoOp
                )
              ]
              [ i
                  [ classy "fa fa-pencil" ]
                  []
              ]
          ]
        td
          [ classy "is-icon" ]
          [ a
              [ voidLinkAction<Actions> ]
              [ i
                  [ classy "fa fa-trash" ]
                  []
              ]
          ]
      ]

  let bodyLoading =
    td
      [ classy "has-text-centered"
        attribute "colspan" "6" ]
      [ i
          [ classy "fa fa-spinner fa-spin" ]
          []
      ]

  let body model =
    tbody
      []
      (
        if model.IsLoading then
          [ bodyLoading ]
        else
          (model.Users |> List.map row)
      )

  let actionArea =
    div
      [ classy "column is-2 is-offset-5" ]
      [ a
          [ classy "button is-primary"
            voidLinkAction<Actions>
            onMouseClick(fun _ ->
              //NavigateTo (Route.User UserApi.Create)
              NoOp
            )
          ]
          [ text "Create a user" ]
      ]


  let view model =
    div
      []
      [ div
          [ classy "columns" ]
          [ actionArea
          ]
        div
          [ classy "columns" ]
          [ table
              [ classy "table" ]
              [ header
                body model
              ]
          ]
      ]

