namespace WebApp

open Fable.Core
open Fable.Import.Browser
open System

open Herebris.SimpleDatabase
open Fable.Core.JsInterop
open WebApp.Common

module Database =

  module Setup =

    let patients =
      [
        { Firstname = "John"
          Surname = "Doe"
          Age = 24
          Email = "john.doe@mail.com"
          Gender = Male
          State = Active
        }
        { Firstname = "Jane"
          Surname = "Doe"
          Age = 27
          Email = "jane.doe@mail.com"
          Gender = Female
          State = Active
        }
        { Firstname = "Barry"
          Surname = "Magium"
          Email = "barry.magium@mail.com"
          Age = 24
          Gender = Male
          State = Inactive
        }
      ]

  let db = new SimpleDatabase()

  let init () =
    db.CreateStore<UserRecord>()

    Setup.patients
    |> List.iter(fun x ->
      db.AddItem<UserRecord> x
    )
