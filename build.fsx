#r "nuget: Fun.Build, 0.3.8"
#r "nuget: Fake.IO.FileSystem, 5.23.1"

open Fun.Build
open Fake.IO
open Fake.IO.Globbing.Operators
open Fake.IO.FileSystemOperators

[<Literal>]
let SRC_DIR = "src"

module Glob =

    let fableJs baseDir =
        baseDir
        </> "**/*.fs.js"

    let fableJsMap baseDir =
        baseDir
        </> "**/*.fs.js.map"

    let js baseDir =
        baseDir
        </> "**/*.js"

    let jsMap baseDir =
        baseDir
        </> "**/*.js.map"

pipeline "Client" {

    stage "Restore dependencies" {
        run "dotnet tool restore"
        run "pnpm install"
    }

    stage "Clean artifacts" {
        paralle

        run(fun _ ->
            [ SRC_DIR </> "bin"; SRC_DIR </> "obj"; SRC_DIR </> "dist" ]
            |> Shell.cleanDirs
        )

        run(fun _ ->
            !!(Glob.fableJs SRC_DIR)
            ++ (Glob.fableJsMap SRC_DIR)
            |> Seq.iter Shell.rm
        )
    }

    stage "Restore dependencies" {
        // This is to avoid errors from the IDE because
        // we deleted the obj and bin folder
        workingDir "src"

        run "dotnet build"
        // Accept exit code 0 and 1 because it can happen that the user
        // relaunches the build when having errors in the code
        acceptExitCodes [ 0; 1 ]
        // Hide the output in the console for the same reason
        // no need to afraid the user with non useful errors
        // If there are errors, the IDE will show them or the next steps will reveal them
        noStdRedirectForStep
    }

    stage "Watch" {
        workingDir "src"

        whenCmd {
            name "--watch"
            alias "-w"
            description "Watch for changes and rebuild"
        }

        paralle

        run "dotnet fable watch"
        run "npx vite"
    }

    stage "Build" {
        workingDir "src"

        whenNot {
            whenCmd {
                name "--watch"
                alias "-w"
                description "Watch for changes and rebuild"
            }
        }

        run "dotnet fable"
        run "npx vite build"
    }

    runIfOnlySpecified false
}

tryPrintPipelineCommandHelp()
