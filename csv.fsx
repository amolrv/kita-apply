open System
open Microsoft.FSharp.Reflection

module Seq =
    let join (delimiter:string) (xs: seq<'a>) =
        xs
        |> Seq.map (fun it -> it.ToString())
        |> String.concat delimiter

let header (ty:Type) =
    FSharpType.GetRecordFields ty
    |> Seq.map (fun it -> it.Name)
    |> Seq.join ","
let inline private enclose s = "\"" + s.ToString() + "\""
let toCsv (data: seq<'a>) =
    let typeA:Type = typeof<'a>
    let csv =
        data
        |> Seq.map (FSharpValue.GetRecordFields >> Array.toSeq >> (Seq.map enclose) >> Seq.join ",")
        |> Seq.join "\n"
    sprintf "%s\n%s" (header typeA) csv