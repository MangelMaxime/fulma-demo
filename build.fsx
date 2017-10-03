// include Fake libs
#r "./packages/build/FAKE/tools/FakeLib.dll"
#r "System.IO.Compression.FileSystem"

open System
open Fake
open Fake.ReleaseNotesHelper
open Fake.Git
open Fake.YarnHelper

let dotnetcliVersion = "2.0.0"
let mutable dotnetExePath = "dotnet"

let runDotnet dir =
    DotNetCli.RunCommand (fun p -> { p with ToolPath = dotnetExePath
                                            WorkingDir = dir
                                            TimeOut =  TimeSpan.FromHours 12. } )
                                            // Extra timeout allow us to run watch mode
                                            // Otherwise, the process is stopped every 30 minutes by default

Target "InstallDotNetCore" (fun _ ->
    dotnetExePath <- DotNetCli.InstallDotNetSDK dotnetcliVersion
)

Target "Clean" (fun _ ->
    !! "/bin"
    ++ "/obj"
    ++ "src/bin"
    ++ "src/obj"
    ++ "output"
    |> Seq.iter(CleanDir)
)

Target "Install" (fun _ ->
    runDotnet __SOURCE_DIRECTORY__ "restore Demo.sln"
)

Target "BuildDotnet" (fun _ ->
    runDotnet __SOURCE_DIRECTORY__ "build Demo.sln"
)

Target "YarnInstall" (fun _ ->
    Yarn (fun p ->
    { p with
        Command = Install Standard
    })
)

Target "Build" (fun _ ->
    runDotnet __SOURCE_DIRECTORY__ "fable webpack --port free -- -p"
)

Target "Watch" (fun _ ->
    runDotnet __SOURCE_DIRECTORY__ "fable webpack-dev-server --port free"
)

// Where to push generated documentation
let publishBranch = "gh-pages"
let fableRoot   = __SOURCE_DIRECTORY__
let temp        = fableRoot </> "temp"
let docsOuput = fableRoot </> "output"

type CommitMessage = Printf.StringFormat<(string-> string)>

// --------------------------------------------------------------------------------------
// Release Scripts
let publishDocs isAppVeyor githubLink =
    let commitMessage : CommitMessage =
        if isAppVeyor then
            "Update site (%s) from local"
        else
            "Update site (%s) from AppVeyor"

    CleanDir temp
    Repository.cloneSingleBranch "" githubLink publishBranch temp

    CopyRecursive docsOuput temp true |> tracefn "%A"
    StageAll temp
    Git.Commit.Commit temp (sprintf commitMessage (DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")))
    Branches.push temp

Target "PublishDocs" (fun _ ->
    publishDocs false "git@github.com:MangelMaxime/fulma-demo.git"
)

Target "PublishDocsAppVeyor" (fun _ ->
    let accessToken =
        match environVarOrNone "access_token" with
        | Some accessToken -> accessToken
        | None -> failwith "The Nuget API key must be set in a NUGET_KEY environmental variable"

    let githubLink = sprintf "https://%s:x-oauth-basic@github.com/MangelMaxime/fulma-demo.git" accessToken

    publishDocs true githubLink
)

// Build order
"Clean"
    ==> "InstallDotNetCore"
    ==> "Install"
    ==> "BuildDotnet"

"BuildDotnet"
    ==> "YarnInstall"
    ==> "Watch"

"BuildDotnet"
    ==> "YarnInstall"
    ==> "Build"
    ==> "PublishDocs"

// start build
RunTargetOrDefault "Build"
