
// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

(* Useful references
https://docs.microsoft.com/en-us/dotnet/fsharp/tutorials/type-providers/accessing-a-sql-database-entities
https://docs.microsoft.com/en-us/dotnet/fsharp/tutorials/type-providers/accessing-a-sql-database
https://data.lacounty.gov/d/3te6-gtm8/visualization
*)

#r "System.Data.dll"
#r "FSharp.Data.TypeProviders.dll"
#r "System.Data.Linq.dll"
#r "System.Data.Entity.dll"
//#r "System.Data.Entity.dll"
#r @"..\packages\EntityFramework.6.2.0\lib\net45\EntityFramework.dll"
#r @"..\packages\FSharp.Data.2.4.3\lib\net45\FSharp.DAta.dll"
//
//open System
//open System.Data
//open System.Data.Linq
//
//open Microsoft.FSharp.Data.TypeProviders
//open Microsoft.FSharp.Linq
//open FSharp.Data
//
//type dbSchema = SqlEntityConnection<"Data Source=PHILLIPGIVENS\SQLEXPRESS;Initial Catalog=LACountyLatLongs;Integrated Security=SSPI;">
//let db = dbSchema.GetDataContext()
//type LocationRec = dbSchema.ServiceTypes.LocationLatLongs
//
//type InLatLongProvider = CsvProvider<"""C:\Repos\BB\familycodeshare\LAC_DPH\RestaurantDashboard\latlong.csv""">
//let latLongs = InLatLongProvider.Load("""C:\Repos\BB\familycodeshare\LAC_DPH\RestaurantDashboard\latlong.csv""")
//latLongs.Rows 
//|> Seq.iter(fun row ->    
//    let sampleLatLong = 
//        sprintf "POINT(%f %f)" row.GIScoord_x row.GIScoord_y
//        |> System.Data.Spatial.DbGeography.FromText
//
//    let sampleRec = 
//        new LocationRec ( 
//            Facility_Name=row.FACILITY_N,
//            Facility_O=row.FACILITY_O,
//            Site_Address=row.SITE_ADDRE,
//            City=row.CITY,
//            State=row.STATE,
//            Zip=row.ZIP.ToString (),
//            CT10=(row.CT10 |> int |> Nullable<int>),
//            Label=(row.LABEL |> float |> Nullable<float>),
//            GISCoord=sampleLatLong
//            )
//
//    db.LocationLatLongs.AddObject(sampleRec)
//    )
//db.DataContext.SaveChanges()
//
//
//// Enable the logging of database activity to the console.
//db.DataContext.Log <- System.Console.Out
//
//let locationsTbl = db.LacDphLocations
