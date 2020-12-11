#load "load.fsx"
#load "lib.fsx"

open Expecto
open Expecto.Flip

let tests = 
    testList "tests" [
        test "Passing test" {
            Expect.isTrue "Expected truth" true
        }
        test "Failing test " {
            Expect.isNotNull "Expected anything" null
        }
        test "Library file" {
            Lib.hello() |> Expect.equal "no hello?" "hello from lib."
        }
    ]

runTests { defaultConfig with verbosity = Logging.Debug } tests