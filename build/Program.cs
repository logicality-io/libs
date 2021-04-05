using System;
using System.Collections.Generic;
using System.IO;
using static Bullseye.Targets;
using static SimpleExec.Command;
using static Logicality.Bullseye.BullseyeUtils;

const string artifactsDir = "artifacts";
const string clean = "clean";
const string build = "build";
const string test = "test";
const string publish = "publish";
const string sln = "Platform.sln";

Target(clean, () => CleanDirectory(artifactsDir));

Target(build, () => Run("dotnet", $"build {sln} -c Release"));

Target(test, () => Run(
    "dotnet",
    $"test {sln} -c Release --collect:\"XPlat Code Coverage\" --settings build/coverlet-settings.xml"));

var defaultTargets = new List<string>
{
    clean, build, test
};

var projectsToPack = new[] { "bullseye", "configuration", "hosting", "pulumi", "system-extensions", "testing" };

foreach (var project in projectsToPack)
{
    var packableProjects = Directory.GetFiles($"libs/{project}/src/", "*.csproj", SearchOption.AllDirectories);
    var packTarget = $"{project}-pack";
    Target(packTarget, DependsOn(build),
        packableProjects,
        packableProject => Run("dotnet", $"pack {packableProject} -c Release -o {artifactsDir} --no-build"));
    defaultTargets.Add(packTarget);
}

Target(publish, () =>
{
    var packagesToPush = Directory.GetFiles(artifactsDir, "*.nupkg", SearchOption.TopDirectoryOnly);
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

defaultTargets.Add(publish);
Target("default", DependsOn(defaultTargets.ToArray()));

RunTargetsAndExit(args);