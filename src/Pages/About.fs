namespace WebApp.Pages

open Fable.Core
open Fable.Import.Browser

open Fable.Arch
open Fable.Arch.App
open Fable.Arch.Html

open WebApp.Common

module About =

  type Model =
    { Value: int
    }

    static member Initial =
      { Value = 0
      }

  type Actions
    = NoOp

  let update model action =
    match action with
    | NoOp -> model, []

  let view model =
    div
      []
      [ text "About" ]
