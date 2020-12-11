#r "nuget: FSharp.Compiler.Service"
open FSharp.Compiler.Interactive.Shell
open System
open System.IO
open System.Text

let sbOut = StringBuilder()
let sbErr = StringBuilder()
let inStream = new StringReader("")
let outStream = new StringWriter(sbOut)
let errStream = new StringWriter(sbErr)

// Build command line arguments & start FSI session
let argv = [| "dotnet fsi" |]
let allArgs = Array.append argv [|"--noninteractive"|]

let fsiConfig = FsiEvaluationSession.GetDefaultConfiguration()
let fsiSession = FsiEvaluationSession.Create(fsiConfig, allArgs, inStream, outStream, errStream)

let eval(scriptFile) =
    try
        let lines = IO.File.ReadAllLines(scriptFile) |> Array.filter (fun s -> s.StartsWith("#load") |> not)
        fsiSession.EvalInteraction (String.Join('\n', lines))
    with ex -> 
        printfn "%s" (sbErr.ToString())
        sbErr.Clear() |> ignore
        
let handleWatcherEvents (e:IO.FileSystemEventArgs) =
    let fi = IO.FileInfo e.FullPath
    printfn "'%A' %A" (fi.Name) (e.ChangeType)
    if fi.Attributes.HasFlag IO.FileAttributes.Hidden ||
       fi.Attributes.HasFlag IO.FileAttributes.Directory ||
       fi.Name = "watch.fsx"
       then ()
    elif fi.Name = "load.fsx" then
        fsiSession.EvalScriptNonThrowing e.FullPath |> ignore
    else
        eval(e.FullPath) |> ignore

let srcDir = __SOURCE_DIRECTORY__
let watcher = new IO.FileSystemWatcher(srcDir, "*.fsx")
watcher.EnableRaisingEvents <- true
watcher.IncludeSubdirectories <- true
watcher.Changed.Add(handleWatcherEvents)
printfn "load script"
fsiSession.EvalScriptNonThrowing "load.fsx" |> ignore
printfn "run script"
eval("run.fsx") |> ignore
System.Console.ReadKey() |> ignore