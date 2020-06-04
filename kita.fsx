#r "nuget: MailKit"
#r "nuget: FSharp.Data"

open Microsoft.FSharp.Core
open FSharp.Data
type Kita = {
        Id: int
        Link: string
        Name: string
        Address: string
        Email: string
        Status: string
        Note: string
    }

type KitaSearchInfo = JsonProvider<const(__SOURCE_DIRECTORY__ + "/search.json")>
type KitaInfo = JsonProvider<const(__SOURCE_DIRECTORY__ + "/kita.json")>
let private site = "https://kita-navigator.berlin.de"
let private baseUrl = sprintf "%s/api/v1/kitas" site

let private seachKita rad lat lon =
    sprintf "%s/umkreissuche?entfernung=%d&lat=%.7f&lon=%.7f&&seite=0&max=1000" baseUrl rad lat lon
    |> Http.RequestString
    |> KitaSearchInfo.Parse

let private fetchKitaInfo (id: int) =
    id
    |> string
    |> sprintf "%s/%s" baseUrl
    |> Http.RequestString
    |> KitaInfo.Parse

let private oneLiner (it:KitaInfo.Adresse) =
    sprintf "%s %s, %d %s" it.Strasse it.Hausnummer it.Plz it.Ort

let private toKita (info:KitaInfo.Root): Kita = {
    Id = info.Einrichtungsauszug.Id
    Link = sprintf "%s/einrichtungen/%d" site info.Einrichtungsauszug.Id
    Name = info.Einrichtungsauszug.Name
    Address = info.Einrichtungsauszug.Adresse |> oneLiner
    Email = info.Kontaktdaten.Emailadresse
    Status = ""
    Note = ""
}

let findKita =
    let kitaId (it:KitaSearchInfo.Einrichtungen) =  it.Id
    let info (it:KitaSearchInfo.Root): KitaSearchInfo.Einrichtungen[] = it.Einrichtungen
    fun rad lat lon ->
        seachKita rad lat lon
        |> info
        |> Array.map (kitaId >> fetchKitaInfo >> toKita)

