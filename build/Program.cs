using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Bullseye.Targets;
using static SimpleExec.Command;
using static Logicality.Bullseye.BullseyeUtils;

const string ArtifactsDir = "artifacts";
const string Clean = "clean";
const string Build = "build";
const string Test = "test";
const string Publish = "publish";
const string Solution = "Platform.sln";

Target(Clean, () => CleanDirectory(ArtifactsDir));

Target(Build, () => Run("dotnet", $"build {Solution} -c Release"));

var defaultTargets = new List<string>
{
    Clean, Build, Test
};

var ignore = new[] {".github"};
var libs = Directory.GetDirectories("libs")
    .Where(d => !ignore.Contains(d))
    .Select(d => d.Replace("libs\\", ""));

foreach (var lib in libs)
{
    var testProjects = Directory.GetFiles($"libs/{lib}/test/", "*.csproj", SearchOption.AllDirectories);
    var testTarget       = $"{lib}-test";
    Target(testTarget, DependsOn(Build),
        testProjects,
        p => Run("dotnet", $"pack {p} -c Release -o {ArtifactsDir} --no-build"));
    defaultTargets.Add(testTarget);

    var packableProjects = Directory.GetFiles($"libs/{lib}/src/", "*.csproj", SearchOption.AllDirectories);
    var packTarget       = $"{lib}-pack";
    Target(packTarget, DependsOn(Build),
        packableProjects,
        packableProject => Run("dotnet", $"pack {packableProject} -c Release -o {ArtifactsDir} --no-build"));
    defaultTargets.Add(packTarget);
}

/*var projectsToPack = new[]
{
    "aspnet-core",
    "bullseye",
    "configuration",
    "hosting",
    "lambda",
    "pulumi",
    "system-extensions",
    "testing"
};

foreach (var project in projectsToPack)
{
    var packableProjects = Directory.GetFiles($"libs/{project}/src/", "*.csproj", SearchOption.AllDirectories);
    var packTarget = $"{project}-pack";
    Target(packTarget, DependsOn(Build),
        packableProjects,
        packableProject => Run("dotnet", $"pack {packableProject} -c Release -o {ArtifactsDir} --no-build"));
    defaultTargets.Add(packTarget);
}*/

Target(Publish, () =>
{
    var packagesToPush = Directory.GetFiles(ArtifactsDir, "*.nupkg", SearchOption.TopDirectoryOnly);
    Console.WriteLine($"Found packages to publish: {string.Join("; ", packagesToPush)}");

    var apiKey = Environment.GetEnvironmentVariable("FEEDZ_LOGICALITY_API_KEY");
    if (string.IsNullOrWhiteSpace(apiKey))
    {
        Console.WriteLine("Feedz API Key not available. No packages will be pushed.");
        return;
    }
    Console.WriteLine($"Feedz API Key ({apiKey.Substring(0, 5)}) available. Pushing packages to Feedz...");
    foreach (var packageToPush in packagesToPush)
    {
        Run("dotnet", $"nuget push {packageToPush} -s https://f.feedz.io/logicality/public/nuget/index.json -k {apiKey} --skip-duplicate", noEcho: true);
    }
});

defaultTargets.Add(Publish);
Target("default", DependsOn(defaultTargets.ToArray()));

RunTargetsAndExit(args);