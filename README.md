# fsiPavlov
Superfast watcher for fsi.

This is an idea I got when solving Advent of Code 2020. I wanted to run unit tests as fast as possible.

The FSharp.Compiler.Service reload fsx-scripts when they change.

## Requirements

dotnet 5

## How to use:
run in a terminal: 
``` bash
dotnet fsi watch.fsx
```

Now change the run.fsx in an editor and watch it fly!
Change #load to add some more nuget packages – run.fsx still flies!

## How does it work
1. `watch.fsx` starts an interactive session and a filewatcher.
2. The filewatcher looks for \*.fsx-files.
3. Changes in files:
   * When a fsx-file change `watch.fsx` will strip the file of `#load`-statements and send it to the interactive session.
   * When `load.fsx` change it will be sent to the interactive mode (takes time because it loads stuff from nuget or does other slow stuff)

## Why? 
Its really really fast: The feedback-loop is as fast as inside a repl.

## Caveats
Everything you do is kept inside the fsi session. variables will not be cleaned up, so there will be ghost hangig around like in the repl. This is  of course solveable, but then the session have to restart and the loop will be slow(er) again.
