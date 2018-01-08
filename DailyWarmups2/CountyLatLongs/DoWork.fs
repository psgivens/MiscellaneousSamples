module DoWork

// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open System
open System.Data
open System.Data.Linq

open Microsoft.FSharp.Data.TypeProviders
open Microsoft.FSharp.Linq
open FSharp.Data


type private dbSchema = SqlEntityConnection<"Data Source=PHILLIPGIVENS\SQLEXPRESS;Initial Catalog=LACountyLatLongs;Integrated Security=SSPI;">
type private dbDataSchema = SqlDataConnection<"Data Source=PHILLIPGIVENS\SQLEXPRESS;Initial Catalog=LACountyLatLongs;Integrated Security=SSPI;">
type private LocationRec = dbSchema.ServiceTypes.LocationLatLongs
type private InLatLongProvider = CsvProvider<"""C:\Repos\BB\familycodeshare\LAC_DPH\RestaurantDashboard\geocoded_deduped.csv""">

let ingestLocations () =
    let db = dbSchema.GetDataContext()

    // Enable the logging of database activity to the console.
    //db.DataContext.Log <- System.Console.Out
    
    let latLongs = InLatLongProvider.Load("""C:\Repos\BB\familycodeshare\LAC_DPH\RestaurantDashboard\geocoded_deduped.csv""")
    latLongs.Rows 
    |> Seq.iter(fun row ->  
        try  
            let sampleLatLong = 
                System.Data.Spatial.DbGeography.PointFromText(
                    (sprintf "POINT(%f %f)" row.GIScoord_x row.GIScoord_y),
                    4326)

            let sampleRec = 
                new LocationRec ( 
                    Record_Id=row.RECORD_ID,
                    Facility_Name=row.FACILITY_NAME,
                    Facility_O=row.OWNER_ID,
                    GISCoord_x=float row.GIScoord_x,
                    GISCoord_y=float row.GIScoord_y,
                    GISCoord=sampleLatLong
                    )

            db.LocationLatLongs.AddObject(sampleRec)
        with
         | :? System.Reflection.TargetInvocationException as ex -> ()
        )
    db.DataContext.SaveChanges() |> ignore

type private AddressRec = dbSchema.ServiceTypes.LacDphLocations
type private InRestaurant = CsvProvider<"""E:\Data\lacdph\deduped.csv""">

let ingestRestaurants () =
    let db = dbSchema.GetDataContext()

    // Enable the logging of database activity to the console.
    //db.DataContext.Log <- System.Console.Out
    
    let restaurants = InRestaurant.Load(@"E:\Data\lacdph\deduped.csv")
    restaurants.Rows 
    |> Seq.iter(fun row ->    
        let sampleRec = 
            new AddressRec ( 
                record_id=row.Record_id,
                address= row.Location_1,
                site_address=row.Location_2,
                site_city=row.Site_city,
                site_zip=row.Site_zip
                )

        db.LacDphLocations.AddObject(sampleRec)
        )
    db.DataContext.SaveChanges() |> ignore


let generateProximalLists recordId =
    use writer = new System.IO.StreamWriter (recordId + ".txt")

    let db = dbDataSchema.GetDataContext()    
    let result = db.SP_SelectNearRestaurants(Nullable<int> 1, recordId) |> Seq.toList
    
    sprintf "{record_id:%A, values:[" recordId
    |> writer.Write 

    result 
    |> Seq.map (fun x -> sprintf "%A" x.Record_Id)
    |> Array.ofSeq
    |> fun a -> String.Join (",", a)
    |> writer.Write

    writer.Write "]}"

let generateProximalJs recordId =
    use writer = new System.IO.StreamWriter (recordId + ".js")
    let db = dbDataSchema.GetDataContext()    
    let result = db.SP_SelectNearRestaurants(Nullable<int> 1, recordId) |> Seq.toList
    
    sprintf "nearByRecords[%A] = {record_id:%A, distance:%d, values:[" recordId recordId 5
    |> writer.Write 

    result 
    |> Seq.map (fun record -> sprintf "%A" record.Record_Id ) 
    |> Array.ofSeq
    |> fun a -> String.Join (",", a)    
    |> writer.Write

    writer.Write "]};\n"

    result
    |> Seq.map (fun record ->
        sprintf "recordLocations[%A] = [%f,%f];\n" record.Record_Id record.X.Value record.Y.Value)
    |> Seq.iter writer.Write 



