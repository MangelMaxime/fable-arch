namespace WebApp

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Import.Browser

open Fable.Arch
open Fable.Arch.App
open Fable.Arch.App.AppApi
open Fable.Arch.Html
open Fable.Arch.RouteParser.Parsing
open Fable.Arch.RouteParser.RouteParser

open Aether
open Aether.Operators

open WebApp
open WebApp.Common

open System

module Main =

  type SubModels =
    { Navbar: Navbar.Model
      Header: Header.Model
      Menu: Menu.Model
      Index: Pages.Index.Model option
      About: Pages.About.Model option
      User: Pages.User.Dispatcher.Model option
      Docs: Pages.Docs.Dispatcher.Model
      Sample: Pages.Sample.Dispatcher.Model option
    }

    static member Initial =
      { Navbar = Navbar.Model.Initial(Index)
        Header = Header.Model.Initial(Index)
        Menu = Menu.Model.Initial(Index)
        Index = None
        About = None
        User = None
        Docs = Pages.Docs.Dispatcher.Model.Initial (DocsApi.Index)
        Sample = None
      }

    static member Index_ =
      (fun r -> r.Index), (fun v r -> { r with Index = Some v } )

    static member Header_ =
      (fun r -> r.Header), (fun v r -> { r with Header = v } )

    static member About_ =
      (fun r -> r.About), (fun v r -> { r with About = Some v } )

    static member User_ =
      (fun r -> r.User), (fun v r -> { r with User = Some v } )

    static member Docs_ =
      (fun r -> r.Docs), (fun v r -> { r with Docs = v } )

    static member Sample_ =
      (fun r -> r.Sample), (fun v r -> { r with Sample = Some v } )

    static member Navbar_ =
      (fun r -> r.Navbar), (fun v r -> { r with Navbar = v } )

    static member Menu_ =
      (fun r -> r.Menu), (fun v r -> { r with Menu = v } )

  type Model =
    { CurrentPage: Route
      SubModels: SubModels
    }

    static member Initial =
      { CurrentPage = Index
        SubModels = SubModels.Initial
      }

    static member SubModels_ =
      (fun r -> r.SubModels), (fun v r -> { r with SubModels = v } )

    static member CurrentPage_ =
      (fun r -> r.CurrentPage), (fun v r -> { r with CurrentPage = v } )


  type Actions
    = NoOp
    | NavigateTo of Route
    | IndexActions of Pages.Index.Actions
    | AboutActions of Pages.About.Actions
    | UserDispatcherAction of Pages.User.Dispatcher.Actions
    | SampleDispatcherAction of Pages.Sample.Dispatcher.Actions
    | DocsDispatcherAction of Pages.Docs.Dispatcher.Actions
    | MenuActions of Menu.Actions
    | HeaderActions of Header.Actions
    | NavbarActions of Navbar.Actions


  let update model action =
    match action with
    | NavigateTo route ->
      match route with
      | Index ->
          let m' =
            model
            |> Optic.set (Model.SubModels_ >-> SubModels.Index_) Pages.Index.Model.Initial
            |> Optic.set (Model.SubModels_ >-> SubModels.Header_ >-> Header.Model.CurrentPage_) route
            |> Optic.set (Model.SubModels_ >-> SubModels.Menu_ >-> Menu.Model.CurrentPage_) route
            |> Optic.set (Model.SubModels_ >-> SubModels.Navbar_ >-> Navbar.Model.CurrentPage_) route
            |> Optic.set (Model.CurrentPage_) route
          m', []
      | About ->
          let m' =
            model
            |> Optic.set (Model.SubModels_ >-> SubModels.About_) Pages.About.Model.Initial
            |> Optic.set (Model.SubModels_ >-> SubModels.Header_ >-> Header.Model.CurrentPage_) route
            |> Optic.set (Model.SubModels_ >-> SubModels.Menu_ >-> Menu.Model.CurrentPage_) route
            |> Optic.set (Model.SubModels_ >-> SubModels.Navbar_ >-> Navbar.Model.CurrentPage_) route
            |> Optic.set (Model.CurrentPage_) route
          m', []
      | Docs subRoute ->
          // Info: We don't set a new value for SubModels.Docs because the value is set at the start of the app
          // and we want to keep the Docs value in "memory" to avoid get doc markdown files severals times
          let m' =
            model
            |> Optic.set (Model.SubModels_ >-> SubModels.Header_ >-> Header.Model.CurrentPage_) route
            |> Optic.set (Model.SubModels_ >-> SubModels.Menu_ >-> Menu.Model.CurrentPage_) route
            |> Optic.set (Model.SubModels_ >-> SubModels.Navbar_ >-> Navbar.Model.CurrentPage_) route
            |> Optic.set (Model.CurrentPage_) route

          let message =
            match subRoute with
            | DocsApi.Index -> []
            | DocsApi.Viewer fileName ->
              [ fun h ->
                  h (DocsDispatcherAction (Pages.Docs.Dispatcher.ViewerActions (Pages.Docs.Viewer.SetDoc fileName)))
              ]

          m', message
      | Sample subRoute ->
          let m' =
            model
            |> Optic.set (Model.SubModels_ >-> SubModels.Sample_) (Pages.Sample.Dispatcher.Model.Initial(subRoute))
            |> Optic.set (Model.SubModels_ >-> SubModels.Header_ >-> Header.Model.CurrentPage_) route
            |> Optic.set (Model.SubModels_ >-> SubModels.Menu_ >-> Menu.Model.CurrentPage_) route
            |> Optic.set (Model.SubModels_ >-> SubModels.Navbar_ >-> Navbar.Model.CurrentPage_) route
            |> Optic.set (Model.CurrentPage_) route

          m', []
    | IndexActions act ->
        let (res, action) = Pages.Index.update model.SubModels.Index.Value act
        let action' = mapActions IndexActions action
        let m' = Optic.set (Model.SubModels_ >-> SubModels.Index_) res model
        m', action'
    | AboutActions act ->
        let (res, action) = Pages.About.update model.SubModels.About.Value act
        let action' = mapActions AboutActions action
        let m' = Optic.set (Model.SubModels_ >-> SubModels.About_) res model
        m', action'
    | UserDispatcherAction act ->
        let (res, action) = Pages.User.Dispatcher.update model.SubModels.User.Value act
        let action' = mapActions UserDispatcherAction action
        let m' = Optic.set (Model.SubModels_ >-> SubModels.User_) res model
        m', action'
    | DocsDispatcherAction act ->
        let (res, action) = Pages.Docs.Dispatcher.update model.SubModels.Docs act
        let action' = mapActions DocsDispatcherAction action
        let m' = Optic.set (Model.SubModels_ >-> SubModels.Docs_) res model
        m', action'
    | SampleDispatcherAction act ->
        // We need to protect from undefined model here
        // This can occured because we are using a producer to Tick the clock sample every seconds
        if model.SubModels.Sample.IsSome then
          let (res, action) = Pages.Sample.Dispatcher.update model.SubModels.Sample.Value act
          let action' = mapActions SampleDispatcherAction action
          let m' = Optic.set (Model.SubModels_ >-> SubModels.Sample_) res model
          m', action'
        else
          model, []
    | MenuActions act ->
        let (res, action) = Menu.update model.SubModels.Menu act
        let action' = mapActions MenuActions action
        let m' = Optic.set (Model.SubModels_ >-> SubModels.Menu_) res model
        m', action'
    | NavbarActions act ->
        let (res, action) = Navbar.update model.SubModels.Navbar act
        let action' = mapActions NavbarActions action
        let m' = Optic.set (Model.SubModels_ >-> SubModels.Navbar_) res model
        m', action'
    | HeaderActions act ->
        let (res, action) = Header.update model.SubModels.Header act
        let action' = mapActions HeaderActions action
        let m' = Optic.set (Model.SubModels_ >-> SubModels.Header_) res model
        m', action'
    | NoOp -> model, []

  let view model =
    let pageHtml =
      match model.CurrentPage with
      | Index -> Html.map IndexActions (Pages.Index.view model.SubModels.Index.Value)
      | Docs subRoute -> Html.map DocsDispatcherAction (Pages.Docs.Dispatcher.view model.SubModels.Docs subRoute)
      | Sample subRoute -> Html.map SampleDispatcherAction (Pages.Sample.Dispatcher.view model.SubModels.Sample.Value subRoute)
      | About -> Html.map AboutActions (Pages.About.view model.SubModels.About.Value)

    let navbarHtml =
      Html.map NavbarActions (Navbar.view model.SubModels.Navbar)

    let menuHtml =
      Html.map MenuActions (Menu.view model.SubModels.Menu)

    let headerHtml =
      Html.map HeaderActions (Header.view model.SubModels.Header)

    div
      []
      [ div
          [ classy "navbar-bg" ]
          [ div
              [ classy "container" ]
              [ navbarHtml
              ]
          ]
        headerHtml
        pageHtml
      ]

  // Customs helpers over the RouteParser
  let (<?>) p1 p2  =
    p1 .>> pchar '?' .>>. p2

  let (<=.>) p1 p2  =
    p1 >>. pchar '=' >>. p2

  let routes =
    [
      runM (NavigateTo Index) (pStaticStr "/" |> (drop >> _end))
      runM (NavigateTo (Docs DocsApi.Index)) (pStaticStr "/docs" |> (drop >> _end))
      runM1 (fun fileName -> NavigateTo (Docs (DocsApi.Viewer fileName))) ((pStaticStr "/docs") <?> (pStaticStr "fileName") <=.> pString)
      runM (NavigateTo (Sample SampleApi.Clock)) (pStaticStr "/sample/clock" |> (drop >> _end))
      runM (NavigateTo (Sample SampleApi.Counter)) (pStaticStr "/sample/counter" |> (drop >> _end))
      runM (NavigateTo (Sample SampleApi.HelloWorld)) (pStaticStr "/sample/hello-world" |> (drop >> _end))
      runM (NavigateTo (Sample SampleApi.NestedCounter)) (pStaticStr "/sample/nested-counter" |> (drop >> _end))
      runM (NavigateTo About) (pStaticStr "/about" |> (drop >> _end))
    ]


  let mapToRoute route =
    match route with
    | NavigateTo r ->
        resolveRoutesToUrl r
    | _ -> None


  let router = createRouter routes mapToRoute

  let locationHandler =
    {
      SubscribeToChange =
        (fun h ->
            window.addEventListener_hashchange(fun _->
              h(location.hash.Substring 1)
              null
            )
        )

      PushChange =
        (fun s -> location.hash <- s)
    }


  let routerF m = router.Route m.Message

  let tickProducer push =
    window.setInterval((fun _ ->
        push(SampleDispatcherAction (Pages.Sample.Dispatcher.ClockActions (Pages.Sample.Clock.Tick DateTime.Now)))
        null
    ),
        1000) |> ignore
    // Force the first to push to have immediate effect
    // If we don't do that there is one second before the first push
    // and the view is rendered with the Model.init values
    push(SampleDispatcherAction (Pages.Sample.Dispatcher.ClockActions (Pages.Sample.Clock.Tick DateTime.Now)))

  // Ensure database creation
  WebApp.Database.init ()

  createApp Model.Initial view update Virtualdom.createRender
  |> withStartNodeSelector "#app"
  |> withProducer (routeProducer locationHandler router)
  |> withProducer tickProducer
  |> withSubscriber (routeSubscriber locationHandler routerF)
  |> start
  |> ignore

  // Init location
  // If hash is empty go to root
  if location.hash = "" then
    location.hash <- "/"
  else
    // Else trigger hashchange to navigate to current route
    window.dispatchEvent(Event.Create("hashchange") ) |> ignore


  [<Emit("Prism.languages.fsharp")>]
  let prismFSharp = ""

  // Configure markdown parser
  let options =
    createObj [
      "highlight" ==> fun code -> PrismJS.Globals.Prism.highlight(code, unbox prismFSharp)
      "langPrefix" ==> "language-"
    ]

  Marked.Globals.marked.setOptions(unbox options)
  |> ignore
