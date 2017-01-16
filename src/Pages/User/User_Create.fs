namespace WebApp.Pages.User

open Fable.Core
open Fable.Import.Browser

open Fable.Arch
open Fable.Arch.App
open Fable.Arch.Html

open WebApp.Common
open WebApp.Common.VDom

module Create =

  type Model =
    { Value: int
    }

    static member Initial =
      { Value = 0
      }

  type Actions =
    | NoOp
    | ChangeFirtname of string

  let update model action =
    match action with
    | NoOp -> model, []
    | ChangeFirtname value ->
        console.log value
        model, []


  let view model =
    div
      []
      [ Html.formInput (InputInfo<_>.Create("Firstname", "Firstname", "coucou", ChangeFirtname))
      ]
