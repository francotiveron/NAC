#if INTERACTIVE
#r "FSharp.Data.TypeProviders.dll"
#endif

open System
open Microsoft.FSharp.Data.TypeProviders

type dbScheme = SqlDataConnection<"Data Source=UNITYVM2\SQLEXPRESS;Initial Catalog=CitAlmHist;Integrated Security=True;Pooling=False">
let db = dbScheme.GetDataContext()

let table = db.Table

let newRecord = new dbScheme.ServiceTypes.Table(AlarmTag = "Alarm", Time = DateTime.Now, Event = 1uy)

db.Table.InsertOnSubmit newRecord

