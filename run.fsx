#load "load.fsx"
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
    ]

runTests { defaultConfig with verbosity = Logging.Debug } tests