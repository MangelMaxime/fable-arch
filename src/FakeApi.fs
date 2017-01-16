namespace WebApp

open Fable.Core
open Fable.Import.Browser
open WebApp.Common

module FakeApi =

  type UserRes
    = Index
    | Show
    | Create
    | Delete

  type Ressources
    = User of UserRes

  [<PassGenerics>]
  let Get<'T> x cb =
    let fire x =
      x
      |> unbox<'T>
      |> cb
    window.setTimeout(
      (fun _ ->
        match x with
        | User res ->
          match res with
          | Index ->
              Database.db.GetAll<UserRecord>() |> fire
          | _ -> ()
        null
      ),
      200
    )
    |> ignore
