module Logary.Tests.Misc

open Swensen.Unquote
open Fuchu
open TestDSL
open Fac

open System
open System.IO
open System.Text.RegularExpressions

open NodaTime

open Logary
open Logary.Targets
open Logary.Configuration

// framework API tests

[<Tests>]
let tests =
  testList "Misc" [
    yield testCase "retrieving logger for name" <| fun _ ->
      Fac.withLogary <| fun logary out err ->
        let logger = "a.b.c.d" |> Registry.getLogger logary.registry |> Async.RunSynchronously
        let logger' = "a.b.c.d" |> (logary |> asLogManager).GetLogger
        (because "logging normally" <| fun () ->
          "Hello world" |> Logger.info logger
          "Goodbye cruel world" |> Logger.fatal logger'
          logary |> finaliseLogary
          out.ToString(), err.ToString())
        |> theTuple
          (fun first second ->
            first |> should contain "Hello world" |> thatsIt
            second |> should contain "Goodbye cruel world" |> thatsIt)
        |> thatsIt

    yield testCase "after shutting down no logging happens" <| fun _ ->
      Fac.withLogary <| fun logary out err ->
        let logger = "a.b.c.d" |> Registry.getLogger logary.registry |> Async.RunSynchronously
        (because "logging something, then shutting down" <| fun () ->
          "hi there" |> Logger.info logger
          logary |> Config.shutdown |> Async.RunSynchronously |> ignore
          "after shutdown" |> Logger.info logger
          out.ToString())
        |> should contain "hi there"
        |> should_not contain "after shutdown"
        |> thatsIt

    yield testCase "timing f-n call" <| fun _ ->
      Fac.withLogary <| fun logary stdout stderr ->
//        let logger = "a.b.c.d" |> Registry.getLogger logary.registry |> Async.RunSynchronously
//        let f = Logary.Derived.Metrics.Time.timelvl logger Info (logger.Name) <| fun () ->
//          printfn "doing work ..."
//          42
//        // TODO: assert on Nimrod output
//        f =? 42
          ()
    ]