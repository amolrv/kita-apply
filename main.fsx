#load "kita.fsx"
#load "mail.fsx"
#load "csv.fsx"

open System.IO
open Mail
open Kita
open Csv


let config = SmtpConfig.GetSample()
let prepareEmail =
    let content = File.ReadAllText (__SOURCE_DIRECTORY__ + "/msg.txt")
    fun kita fromEmail fromName ->
        let subject = sprintf "%s | Kita anmeldung" kita.Name
        message content subject kita.Email fromEmail fromName

let applyToKita sendEmail =
    let sendEmailInternal kita =
        let mail = prepareEmail kita config.Sender config.Name
        match (sendEmail mail) with
        | Ok _ -> { kita with Status = "Email Sent"}
        | Error _ -> { kita with Status = "Failed"}
    Array.map (sendEmailInternal)
let emailSender client =
    let result =
        findKita 2 52.507248 13.494900
        |> applyToKita (sendEmail client)
        |> Array.map (fun it -> {it with Status = "Email Sent"})
        |> toCsv
    File.WriteAllText (__SOURCE_DIRECTORY__ + "/result.csv", result)

emailSender |> client config