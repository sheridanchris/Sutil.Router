open Browser
open Sutil
open Sutil.Core
open Sutil.Router
open Sutil.CoreElements
open type Feliz.length

type Page =
  | Home
  | Login
  | UserProfile of userId: int
  | NotFound

let getPageFromUrl (url: string list) =
  match url with
  | [] -> Home
  | [ "users"; Route.Int userId ] -> UserProfile userId
  | [ "login" ] -> Login
  | _ -> NotFound

type Model = { CurrentPage: Page }

let currentPage model = model.CurrentPage

type Msg =
  | Navigate of string
  | SetPage of Page

let init () =
  let currentUrl = Router.getCurrentUrl window.location
  let currentPage = getPageFromUrl currentUrl
  { CurrentPage = currentPage }, Cmd.none

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
  match msg with
  | Navigate path -> model, Router.navigate path
  | SetPage page -> { model with CurrentPage = page }, Cmd.none

let view () =
  let dispose _ = ()
  let model, dispatch = () |> Store.makeElmish init update dispose

  let routerSubscription =
    Navigable.listenLocation (Router.getCurrentUrl, getPageFromUrl >> SetPage >> dispatch)

  Html.div
    [ Attr.style [ Css.displayFlex; Css.flexDirectionColumn ]

      Html.button
        [ Attr.style [ Css.width 350 ]
          Attr.text "Click me to navigate to the login page!"
          onClick (fun _ -> dispatch (Navigate "/#/login")) [] ]
      Html.a [ Attr.href "/#/users/1"; Attr.text "Click me to navigate to a user's profile" ]
      Bind.el (
        model .> currentPage,
        fun currentPage ->
          let content =
            match currentPage with
            | Home -> "Home page"
            | Login -> "Login page"
            | UserProfile id -> $"Viewing user #{id}"
            | NotFound -> "Not found!"

          Html.h1 content
      ) ]

Program.mount ("sutil-app", view ()) |> ignore // Do I ignore this?
