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
const string Push = "push";
const string Solution = "Platform.sln";

Target(Clean, () => CleanDirectory(ArtifactsDir));

Target(Build, () => Run("dotnet", $"build {Solution} -c Release"));

var defaultTargets = new List<string>
{
    Clean, Build
};

var ignore = new[] {".github"};
var libs = Directory.GetDirectories("libs")
    .Where(d => !ignore.Contains(d))
    .Select(d => new DirectoryInfo(d).Name);

foreach (var lib in libs)
{
    var testProjects = Directory.GetFiles($"libs/{lib}/test/", "*.csproj", SearchOption.AllDirectories);
    var testTarget = $"{lib}-test";
    Target(testTarget,
        testProjects,
        p => Run("dotnet", $"pack {p} -c Release -o {ArtifactsDir}"));
    defaultTargets.Add(testTarget);

    var packableProjects = Directory.GetFiles($"libs/{lib}/src/", "*.csproj", SearchOption.AllDirectories);
    var packTarget = $"{lib}-pack";
    Target(packTarget, DependsOn(Clean),
        packableProjects,
        packableProject => Run("dotnet", $"pack {packableProject} -c Release -o {ArtifactsDir}"));
    defaultTargets.Add(packTarget);
}

Target(Push, () =>
{
    var packagesToPush = Directory.GetFiles(ArtifactsDir, "*.nupkg", SearchOption.TopDirectoryOnly);
    Console.WriteLine($"Found packages to publish: {string.Join("; ", packagesToPush)}");

    var apiKey = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
    if (string.IsNullOrWhiteSpace(apiKey))
    {
        Console.WriteLine("GITHUB_TOKEN not available. No packages will be pushed.");
        return;
    }
    Console.WriteLine($"Nuget API Key ({apiKey.Substring(0, 5)}) available. Pushing packages...");
    foreach (var packageToPush in packagesToPush)
    {
        Run("dotnet", $"nuget push {packageToPush} -s https://https://nuget.pkg.github.com/logicality-io/index.json -k {apiKey} --skip-duplicate", noEcho: true);
    }
});

defaultTargets.Add(Push);
Target("default", DependsOn(defaultTargets.ToArray()));

RunTargetsAndExit(args);