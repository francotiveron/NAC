#if INTERACTIVE
#r "FSharp.Data.TypeProviders.dll"
#r "System.Data.Linq.dll"
#endif

open System
open System.IO
open System.Data.Linq

let print e = printfn "%A" e

//let lines = seq { for line in File.ReadLines @"C:\temp\alarm\AlarmLog.txt" do yield line}
let lines = File.ReadLines @"C:\temp\alarm\AlarmLog.txt"

//lines |> Seq.iter (printfn "%A")

//let fieldLength = [17; 33; 59; 105; 117]

let f1 (parts, line:string, from) upTo =
    line.Substring(from, upTo - from).Trim() :: parts, line, upTo

//let splitLine (line:string) = 
//    let (r, _, _) = fieldLength |> List.fold f1 ([], line, 0)
//    List.rev r
let splitLine (line:string) = 
    line.Split('\t') |> Array.map (fun s -> s.Trim())

let l1 = lines |> Seq.map splitLine

//let lt = lines |> Seq.map (fun (line:string) -> line.Split('\t'))

//l1 |> Seq.take 2 |> Seq.iter (printfn "%A")

type entry = {
    AlarmTag : string
    Time : DateTime
    Event : byte
}

let toRecord (li:string []) = {
    AlarmTag = li.[2]
    Time = DateTime.Parse(li.[0] + " " + li.[1])
    Event = 
        match li.[4] with
        | "ON" -> 1uy
        | _ -> 0uy
}


//l1 |> Seq.take 3 |> Seq.map toRecord |> Seq.iter print

open Microsoft.FSharp.Data.TypeProviders
open System.Globalization

type dbScheme = SqlDataConnection<"Data Source=UNITYVM2\SQLEXPRESS;Initial Catalog=CitAlmHist;Integrated Security=True;Pooling= False">
let db = dbScheme.GetDataContext()

let toDT sDT = 
    match DateTime.TryParse(sDT, CultureInfo.GetCultureInfo("en-AU"), DateTimeStyles.None) with
    | (true, dt) -> Some dt
    | (false, _) ->
//        print sDT
        None


let toRecord1 sAlarmTag sDescription dtTime sEvent = 
    new dbScheme.ServiceTypes.Table1(
        AlarmTag = sAlarmTag, 
        Description = sDescription,
        Time = dtTime,
        Event = match sEvent with
                | "ON" -> 1uy
                | _ -> 0uy)


//let l2 = l1 |> Seq.chunkBySize 1000
//
//l2
//|> Seq.iter (fun grp ->
//                grp 
//                |> Seq.iter (fun li -> 
//                                match toDT (li.[0] + " " + li.[1]) with
//                                | Some t -> db.Table1.InsertOnSubmit(toRecord1 li.[2] t li.[4])
//                                | _ -> ())
//                db.DataContext.SubmitChanges())

l1
|> Seq.distinctBy (fun li -> li.[0] + li.[1] + li.[2] + li.[4])
|> Seq.choose (fun li -> 
    match toDT (li.[0] + " " + li.[1]) with
    | Some t -> Some (toRecord1 li.[2] li.[3] t li.[4])
    | _ -> None) 
//|> Seq.take 5
|> db.Table1.InsertAllOnSubmit 
//|> Seq.iter (fun li -> 
//    match toDT (li.[0] + " " + li.[1]) with
//    | Some t -> db.Table1.InsertOnSubmit(toRecord1 li.[2] t li.[4])
//    | _ -> ())
//|> Seq.iter (fun e -> print e.Time)
//|> Seq.skip 200
//|> Seq.take 10000
//|> Seq.iter (fun e -> 
//                db.Table1.InsertOnSubmit(e) 
//                db.DataContext.SubmitChanges())

db.DataContext.SubmitChanges()
//try db.DataContext.SubmitChanges() 
//with :? System.Data.Linq.DuplicateKeyException as x -> print x.Data

