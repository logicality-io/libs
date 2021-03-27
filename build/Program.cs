using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Bullseye.Targets;
using static SimpleExec.Command;

const string artifactsDir = "artifacts";
const string clean = "clean";
const string build = "build";
const string publish = "publish";

Target(clean, () =>
{
    if (Directory.Exists(artifactsDir))
    {
        Directory.Delete(artifactsDir, true);
    }
    Directory.CreateDirectory(artifactsDir);
});

Target(build, () => Run("dotnet", "build libs/hosting/Hosting.sln -c Release"));

var projects = new[] {"configuration", "hosting"};
var defaultTargets = new List<string>()
{
    clean
};

foreach (var project in projects)
{
    var sln = Directory.GetFiles($"libs/{project}/", "*.sln").Single();
    var buildTarget = $"{project}-build";
    Target(buildTarget, () => Run("dotnet", $"build {sln} -c Release"));
    defaultTargets.Add(buildTarget);

    var testTarget = $"{project}-test";
    Target(testTarget, DependsOn(buildTarget),
        () => Run("dotnet",
            $"test {sln} -c Release --no-build -r {artifactsDir} " +
            $"--collect:\"XPlat Code Coverage\" --settings build/coverlet-settings.xml"));
    defaultTargets.Add(testTarget);

    var packableProjects = Directory.GetFiles($"libs/{project}/src/", "*.csproj", SearchOption.AllDirectories);
    var packTarget = $"{project}-pack";
    Target(packTarget, DependsOn(buildTarget),
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