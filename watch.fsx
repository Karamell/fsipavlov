#r "nuget: FSharp.Compiler.Service"
open FSharp.Compiler.Interactive.Shell
open System
open System.IO
open System.Text

let [<Literal>] runScript = "run.fsx"
let [<Literal>] loadScript = "load.fsx"

module Session =
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
            let lines = 
                IO.File.ReadAllLines(scriptFile) 
                |> Array.filter (fun s -> s.StartsWith($"#load \"{loadScript}\"") |> not)
            fsiSession.EvalInteraction (String.Join('\n', lines))
        with ex ->
            printfn "%s" (sbErr.ToString())
            sbErr.Clear() |> ignore
        
let handleWatcherEvents (e:IO.FileSystemEventArgs) =
    let fi = IO.FileInfo e.FullPath
    printfn "'%s' %A" (fi.Name) (e.ChangeType)
    if fi.Attributes.HasFlag IO.FileAttributes.Hidden ||
       fi.Attributes.HasFlag IO.FileAttributes.Directory ||
       fi.Name = __SOURCE_FILE__
       then ()
    elif fi.Name = loadScript then
        Session.fsiSession.EvalScriptNonThrowing e.FullPath |> ignore
    else
        Session.eval(runScript) |> ignore

let watcher = 
    let srcDir = __SOURCE_DIRECTORY__
    let w = new IO.FileSystemWatcher(srcDir, "*.fsx",EnableRaisingEvents = true ,IncludeSubdirectories = true)
    w.Changed.Add(handleWatcherEvents)

printfn "run the load script '%s'." loadScript
Session.fsiSession.EvalScriptNonThrowing loadScript |> ignore
printfn "run startup/run script '%s'." runScript
Session.eval runScript |> ignore
printfn "Ready. Now try and change '%s' ..." runScript
System.Console.ReadKey() |> ignore
printfn "Wow. That was fast!"