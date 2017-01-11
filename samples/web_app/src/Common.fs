namespace WebApp

open Fable.Arch.Html

module Common =

  [<RequireQualifiedAccess>]
  module UserApi =
    type Route
      = Index
      | Create
      | Edit of int
      | Show of int

  [<RequireQualifiedAccess>]
  module DocsApi =
    type Route
      = Index
      | HMR

  [<RequireQualifiedAccess>]
  module SampleApi =
    type Route
      = Clock
      | Counter

  type Route
    = Index
    | Docs of DocsApi.Route
    | User of UserApi.Route
    | Sample of SampleApi.Route
    | About

  let resolveRoutesToUrl r =
    match r with
      | Index -> Some "/"
      | Docs api ->
        match api with
        | DocsApi.Index -> Some "/docs"
        | DocsApi.HMR -> Some "/docs/hmr"
      | Sample api ->
        match api with
        | SampleApi.Clock -> Some "/sample/clock"
        | SampleApi.Counter -> Some "/sample/counter"
      | About -> Some "/about"
      | User api ->
        match api with
        | UserApi.Index -> Some "/users"
        | UserApi.Create -> Some "/user/create"
        | UserApi.Edit id -> Some (sprintf "/user/%i/edit" id)
        | UserApi.Show id -> Some (sprintf "/user/%i" id)

  let voidLinkAction<'T> : Attribute<'T> = attribute "href" "javascript:void(0)"

  [<AutoOpen>]
  module Types =

    type Gender =
      | Male
      | Female

      override self.ToString () =
        match self with
        | Male -> "Mr."
        | Female -> "Mrs."

    type State =
      | Active
      | Inactive

    type UserRecord =
      { Firstname: string
        Surname: string
        Age: int
        Email: string
        Gender: Gender
        State: State
      }

  module VDom =

    [<AutoOpen>]
    module Types =

      type LabelInfo =
        { RefId: string
          Text: string
        }

        static member Create (refId, txt) =
          { RefId = refId
            Text = txt
          }

        member self.fRefId =
          sprintf "f%s" self.RefId

      type InputInfo<'Action> =
        { RefId: string
          Placeholder: string
          Value: string
          Action: string -> 'Action
        }

        static member Create (refId, placeholder, value, action) =
          { RefId = refId
            Placeholder = placeholder
            Value = value
            Action = action
          }

        member self.fRefId =
          sprintf "f%s" self.RefId

        member self.ToLabelInfo txt =
          { RefId = self.RefId
            Text = txt
          }

    module Html =

      open Fable.Core.JsInterop

      let onInput x = onEvent "oninput" (fun e -> x (unbox e?target?value))

      let controlLabel (info: Types.LabelInfo) =
        label
            [ classy "label"
              attribute "for" info.fRefId
            ]
            [ text info.Text ]

      let formInput<'Action> (info: Types.InputInfo<'Action>) =
        div
          []
          [
            controlLabel (info.ToLabelInfo "Firstname")
            p
              [ classy "control" ]
              [ input
                  [ classy "input"
                    attribute "id" info.fRefId
                    attribute "type" "text"
                    attribute "placeholder" info.Placeholder
                    property "value" info.Value
                    onInput (fun x -> info.Action x)
                  ]
              ]
          ]