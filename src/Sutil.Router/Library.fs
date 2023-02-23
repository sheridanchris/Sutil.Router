namespace Sutil.Router

open System
open Sutil
open Browser
open Browser.Types

module internal Common =
  let split (splitBy: string) (value: string) =
    value.Split([| splitBy |], StringSplitOptions.RemoveEmptyEntries)
    |> Array.toList

  let splitRoute = split "/"
  let splitQueryParam = split "&"
  let splitKeyValuePair = split "="

  let getKeyValuePair (parts: 'a list) =
    if List.length parts = 2 then
      Some(parts[0], parts[1])
    else
      None

[<RequireQualifiedAccess>]
module Router =
  open Common

  let getCurrentUrl (location: Location) =
    let route =
      if location.hash.Length > 1 then
        location.hash.Substring 1
      else
        ""

    if route.Contains("?") then
      let routeValues = route.Substring(0, route.IndexOf("?"))
      let queryParams = route.Substring(routeValues.Length + 1)
      splitRoute routeValues @ [ queryParams ]
    else
      splitRoute route

  let navigate (url: string) : Cmd<_> =
    Cmd.ofEffect (fun _ -> window.location.assign url)

// Credits to Feliz.Router for these amazing active pattern definitions.
// https://github.com/Zaid-Ajaj/Feliz.Router/blob/master/src/Router.fs#L1430
module Route =
  open Common

  let (|Int|_|) (value: string) =
    match Int32.TryParse value with
    | true, value -> Some value
    | _ -> None

  let (|Int64|_|) (input: string) =
    match Int64.TryParse input with
    | true, value -> Some value
    | _ -> None

  let (|Guid|_|) (input: string) =
    match Guid.TryParse input with
    | true, value -> Some value
    | _ -> None

  let (|Number|_|) (input: string) =
    match Double.TryParse input with
    | true, value -> Some value
    | _ -> None

  let (|Decimal|_|) (input: string) =
    match Decimal.TryParse input with
    | true, value -> Some value
    | _ -> None

  let (|Bool|_|) (input: string) =
    match input.ToLower() with
    | ("1" | "true") -> Some true
    | ("0" | "false") -> Some false
    | "" -> Some true
    | _ -> None

  let (|Query|_|) (value: string) =
    let queryParams = splitQueryParam value

    if List.length queryParams = 0 then
      None
    else
      queryParams
      |> List.map (splitKeyValuePair >> getKeyValuePair)
      |> List.choose id
      |> Some
