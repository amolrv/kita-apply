#r "nuget: MailKit"
#r "nuget: FSharp.Data"

open MimeKit
open MailKit.Net.Smtp
open FSharp.Data

type SmtpConfig = JsonProvider<const(__SOURCE_DIRECTORY__ + "/smtp.json")>

let client (it: SmtpConfig.Root) =
    fun f ->
        use client = new SmtpClient()
        client.Connect(it.Server, it.Port, true)
        client.Authenticate(it.Sender, it.Password)
        client |> f

let private body content =
     let body = TextPart("plain")
     body.Text <- content
     body

let message (content:string) (subject:string) (toEmail:string) (fromEmail:string) (fromName:string) =
    let mail = MimeMessage()
    mail.Subject <- subject
    mail.Body <- body content
    mail.From.Add(MailboxAddress(fromEmail, fromName))
    mail.To.Add(MailboxAddress(toEmail, toEmail))
    mail

let sendEmail (client:SmtpClient) email =
    try
        client.Send email |> Ok
    with e -> Error e
