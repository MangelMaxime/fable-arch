namespace WebApp

open Fable.Core
open Fable.Import.Browser

open Fable.Arch
open Fable.Arch.App
open Fable.Arch.Html

open WebApp.Common

module Menu =

  type Model =
    { CurrentPage: Route
    }

    static member Initial (page) =
      { CurrentPage = page
      }

    static member CurrentPage_ =
      (fun r -> r.CurrentPage), (fun v r -> { r with CurrentPage = v })

  type Actions
    = NoOp
    | NavigateTo of Route

  type MenuLink =
    { Text: string
      Route: Route
    }

    static member Create (text, route) =
      { Text = text
        Route = route
      }

  let update model action =
    match action with
    | NoOp -> model, []
    | NavigateTo route ->
      let message =
        [ fun h ->
            let url = resolveRoutesToUrl route
            match url with
            | Some u -> location.hash <- u
            | None -> failwith "Cannot be reached. Route should always be resolve"
        ]
      model, message

  let menuListItem menuLink currentPage =
    let class' =
      classList
        [ "is-active", menuLink.Route = currentPage
        ]
    li
      []
      [ a
          [ class'
            voidLinkAction<Actions>
            onMouseClick (fun _ ->
              NavigateTo menuLink.Route
            )
          ]
          [ text menuLink.Text ]
      ]

  let menuList items currentPage =
      ul
        [ classy "menu-list" ]
        (items |> List.map(fun x -> menuListItem x currentPage))

  let menuSection txt =
    p
      [ classy "menu-label" ]
      [ text txt
      ]

  let view model =
    aside
      [ classy "menu" ]
      [ menuSection "Users"
        menuList
          [ //MenuLink.Create("Index", (Route.User UserApi.Index))
            //MenuLink.Create("Create", (Route.User UserApi.Create))
          ]
          model.CurrentPage
      ]
