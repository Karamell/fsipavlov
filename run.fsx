#load "load.fsx"
open Expecto
open Expecto.Flip

let tester = 
    testList "tests" [
        test "Sannhet" {
            Expect.isTrue "Forventa sannhet" true
        }
        test "Hva syns du om dette?" {
            Expect.isFalse "Feil eller?" true
        }   
        test "Hva nå da? " { Expect.isNotNull "Det var jo null!" null}
        test "Hei!" { Expect.isNotNull "Det var jo null!" null}
    ]

runTests { defaultConfig with verbosity = Logging.Debug } tester