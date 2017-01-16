namespace Herebris

open Fable.Core
open Fable.Import.Browser
open System.Collections.Generic

module SimpleDatabase =

  type StoreItem<'T> =
    { Id: int
      Value: 'T
    }

    [<PassGenerics>]
    static member ExtractValue item =
      unbox<'T> item.Value

  type Store<'T> =
    { mutable Rows: StoreItem<'T> list
    }

    static member Create () =
      { Rows = []
      }

  type StoreMeta =
    { Name: string
      mutable Id: int
    }

    static member Create (name) =
      { Name = name
        Id = 0
      }

    member self.NextId =
      let id = self.Id
      self.Id <- self.Id + 1
      id

  type DatabaseRecord =
    { Metadatas: Dictionary<string, StoreMeta>
      Stores: Dictionary<string, Store<obj>>
    }

    static member Create () =
      { Metadatas = new Dictionary<_,_>()
        Stores = new Dictionary<_,_>()
      }

  type SimpleDatabase() =
    let db = DatabaseRecord.Create()

    [<PassGenerics>]
    member self.CreateStore<'S> () =
      let storeName = typeof<'S>.Name
      if not (db.Metadatas.ContainsKey(storeName)) then
        db.Metadatas.Add(storeName, StoreMeta.Create(storeName))
        db.Stores.Add(storeName, Store<'S>.Create())
      else
        failwithf "Store: %s already exist" storeName

    [<PassGenerics>]
    member self.CreateStoreAsync<'S> () =
      let storeName = typeof<'S>.Name
      if not (db.Metadatas.ContainsKey(storeName)) then
        db.Metadatas.Add(storeName, StoreMeta.Create(storeName))
        db.Stores.Add(storeName, Store<'S>.Create())
      else
        failwithf "Store: %s already exist" storeName

    [<PassGenerics>]
    member self.AddItem<'S>(item: 'S) =
      let storeName = typeof<'S>.Name
      try
        db.Stores.[storeName].Rows <-
          { Id = db.Metadatas.[storeName].NextId
            Value = item
          } :: db.Stores.[storeName].Rows
      with
        | _ -> failwithf "Store: %s not found" storeName

    [<PassGenerics>]
    member self.AddItemAsync<'S>(item: 'S) =
      let storeName = typeof<'S>.Name
      try
        db.Stores.[storeName].Rows <-
          { Id = db.Metadatas.[storeName].NextId
            Value = item
          } :: db.Stores.[storeName].Rows
      with
        | _ -> failwithf "Store: %s not found" storeName

    [<PassGenerics>]
    member self.Get<'S>(id) =
      let storeName = typeof<'S>.Name
      try
        try
          db.Stores.[storeName].Rows
          |> List.find(fun x -> x.Id = id)
          |> StoreItem<'S>.ExtractValue
          |> Some
        with
          | _ -> None
      with
        | _ -> failwithf "Store: %s not found" storeName

    [<PassGenerics>]
    member self.GetItems<'S>(action) =
      let storeName = typeof<'S>.Name
      try
        db.Stores.[storeName].Rows
        |> List.filter(fun x -> unbox action)
        |> List.map(fun x -> x.Value)
      with
        | _ -> failwithf "Store: %s not found" storeName

    [<PassGenerics>]
    member self.GetAll<'S>() =
      let storeName = typeof<'S>.Name
      try
        db.Stores.[storeName].Rows
        |> List.map(fun x ->
          StoreItem<'S>.ExtractValue x
        )
      with
        | _ -> failwithf "Store: %s not found" storeName

    [<PassGenerics>]
    member self.Set<'S>(id, item) =
      let storeName = typeof<'S>.Name
      try
        db.Stores.[storeName].Rows
        |> List.map(fun x ->
          if x = id then
            item
          else
            x
        )
      with
        | _ -> failwithf "Store: %s not found" storeName
