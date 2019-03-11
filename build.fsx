#r "paket:
nuget NUnit.ConsoleRunner
nuget Fake.IO.FileSystem
nuget Fake.DotNet.AssemblyInfoFile
nuget Fake.DotNet.MSBuild
nuget Fake.DotNet.Testing.Nunit
nuget Fake.Testing.Common
nuget Fake.DotNet.NuGet
nuget Fake.IO.Zip
nuget Fake.Core.Target //"
#load "./.fake/build.fsx/intellisense.fsx"

open Fake.IO
open Fake.IO.Globbing.Operators //enables !! and globbing
open Fake.Core
open Fake.DotNet.Testing
open Fake.DotNet

//Properties
let buildFolder = System.IO.Path.GetFullPath("./build/")
let buildLibFolder = buildFolder + "lib"
let buildTestFolder = buildFolder + "test"
let artifactFolder = System.IO.Path.GetFullPath("./artifact/")
let artifactAppFolder = artifactFolder + "app"
let nugetFolder = "./NuGet/"

let globalPackagesFolder =     
    System.Environment.ExpandEnvironmentVariables("%userprofile%\.nuget\packages")

let assemblyVersion =
    let majorVersion = "1"
    let minorVersion = "0"
    let now = System.DateTime.Now    
    let buildVersion = sprintf "%02d%03d" (now.Year - 2000) (now.DayOfYear) //Example: 19063
    let revisionVersion = "2"
    sprintf "%s.%s.%s.%s" majorVersion minorVersion buildVersion revisionVersion

let getVersion file = 
    System.Reflection.AssemblyName.GetAssemblyName(file).Version.ToString()

let nuspecTemplate () =
    !! "src/lib/**/Win32.TokenPrivileges.nuspec"
    |> Seq.head

let nuspecTarget =
    System.IO.Path.Combine(buildLibFolder,"Win32.TokenPrivileges.nuspec")    

let copyPackFiles () =
    !! "build/lib/**/Win32.TokenPrivileges.dll"
    ++ "build/lib/**/Win32.TokenPrivileges.pdb"    
    |> Shell.copy "Nuget"        

let sourceFiles () =
    !! "src/**/*.*"
    -- "**/bin/**"
    -- "**/obj/**"
    -- "**/test/**"
    -- "**/.vs/**"

//Targets
Target.create "Clean" (fun _ ->
    Trace.trace "Clean folders..."
    Shell.cleanDirs [ buildFolder; artifactFolder; nugetFolder ]
)

Target.create "RestorePackages" (fun _ ->
     "./Win32.TokenPrivileges.sln"
     |> Fake.DotNet.NuGet.Restore.RestoreMSSolutionPackages (fun p ->
         { p with             
             Retries = 4 })
   )

Target.create "BuildLib" (fun _ -> 
    Trace.trace "Building lib..."    
    AssemblyInfoFile.createCSharp "./src/lib/Win32.TokenPrivileges/Properties/AssemblyInfo.cs"
        [
            AssemblyInfo.Title "Win32.TokenPrivileges"
            AssemblyInfo.Description "Adjust token privileges of a Win32 process" 
            AssemblyInfo.Product "Win32.TokenPrivileges"
            AssemblyInfo.Company "github/trondr"
            AssemblyInfo.Copyright "Copyright \u00A9 github/trondr 2019"
            AssemblyInfo.Version assemblyVersion
            AssemblyInfo.FileVersion assemblyVersion                        
            AssemblyInfo.ComVisible false
            AssemblyInfo.Guid "59f9fa7c-b4a9-422c-8c52-958bbd1c6688"
            AssemblyInfo.InternalsVisibleTo "Win32.TokenPrivileges.Tests"
        ]
    !! "src/lib/**/*.csproj"
        |> Fake.DotNet.MSBuild.runRelease id buildLibFolder "Build"
        |> Trace.logItems "BuildApp-Output: "
)

Target.create "BuildTest" (fun _ -> 
    Trace.trace "Building test..."
    !! "src/test/**/*.csproj"
        |> Fake.DotNet.MSBuild.runRelease id buildTestFolder "Build"
        |> Trace.logItems "BuildTest-Output: "
)

let nunitConsoleRunner() =
    let consoleRunner = 
        let search = 
            !! (globalPackagesFolder + "/**/nunit3-console.exe")
        if (Seq.isEmpty search) then 
            (failwith "Could not locate nunit3-console.exe in ./packages folder.")
        else 
            (search |> Seq.head)        
    printfn "Console runner:  %s" consoleRunner
    consoleRunner

Target.create "Test" (fun _ -> 
    Trace.trace "Testing app..."    
    !! ("build/test/**/*.Tests.dll")    
    |> NUnit3.run (fun p ->
        {p with ToolPath = nunitConsoleRunner();Where = "cat==UnitTests";TraceLevel=NUnit3.NUnit3TraceLevel.Verbose})
)

let setNugetParameters (nugetParams:Fake.DotNet.NuGet.NuGet.NuGetParams) =
    let nparams = {nugetParams with Authors=["github.com/trondr"];Version=assemblyVersion;SymbolPackage=Fake.DotNet.NuGet.NuGet.NugetSymbolPackage.Nuspec}
    printfn "NuGet Paramaters: %A" nparams
    nparams

Target.create "CreateNugetPackage" (fun _ ->
    Trace.trace "Creating nuget package..."
    copyPackFiles ()
    sourceFiles() |> Shell.copyFilesWithSubFolder "NuGet"
    System.IO.File.Copy(nuspecTemplate(),nuspecTarget,true)
    Fake.DotNet.NuGet.NuGet.NuGetPack setNugetParameters nuspecTarget    
)

Target.create "Publish" (fun _ ->
    Trace.trace "Publishing app..."
    let assemblyVersion = getVersion (System.IO.Path.Combine(buildLibFolder,"Win32.TokenPrivileges.dll"))
    let files = 
        [|
            System.IO.Path.Combine(buildLibFolder,"Win32.TokenPrivileges.dll")
            System.IO.Path.Combine(buildLibFolder,"Win32.TokenPrivileges.pdb")
        |]
    let zipFile = System.IO.Path.Combine(artifactFolder,sprintf "Win32.TokenPrivileges.%s.zip" assemblyVersion)
    files
    |> Fake.IO.Zip.createZip buildLibFolder zipFile (sprintf "Win32.TokenPrivileges %s" assemblyVersion) 9 false 
)

Target.create "Default" (fun _ ->
    Trace.trace "Hello world from FAKE"
)

//Dependencies
open Fake.Core.TargetOperators

"Clean" 
    ==> "RestorePackages"
    ==> "BuildLib"
    ==> "BuildTest"
    ==> "Test"
    ==> "CreateNugetPackage"
    ==> "Publish"
    ==> "Default"

//Start build
Target.runOrDefault "Default"