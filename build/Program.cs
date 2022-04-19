using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Bullseye.Targets;
using static SimpleExec.Command;
using static Logicality.Bullseye.BullseyeUtils;

const string ArtifactsDir   = "artifacts";
const string Clean          = "clean";
const string Build          = "build";
const string PushToGitHub   = "push-github";
const string PushToNugetOrg = "push-nugetorg";
const string Solution       = "PlatformLibs.sln";

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

Target(PushToGitHub, () =>
{
    var apiKey = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
    if (string.IsNullOrWhiteSpace(apiKey))
    {
        Console.WriteLine("GITHUB_TOKEN not available. No packages will be pushed.");
        return;
    }
    Console.WriteLine($"Nuget API Key ({apiKey.Substring(0, 5)}) available. Pushing packages...");
    Run("dotnet", $"nuget push \"{ArtifactsDir}\\*.nupkg\" -s https://nuget.pkg.github.com/logicality-io/index.json -k {apiKey} --skip-duplicate", noEcho: true);
});
defaultTargets.Add(PushToGitHub);

Target(PushToNugetOrg, () =>
{
    var apiKey = Environment.GetEnvironmentVariable("NUGETORG_API_KEY");
    if (string.IsNullOrWhiteSpace(apiKey))
    {
        Console.WriteLine("NUGETORG_API_KEY not available. No packages will be pushed.");
        return;
    }
    Console.WriteLine($"Nuget API Key ({apiKey.Substring(0, 5)}) available. Pushing packages..."); 
    Run("dotnet", $"nuget push artifacts\\*.nupkg -s https://api.nuget.org/v3/index.json -k {apiKey} --skip-duplicate", noEcho: true);
});
defaultTargets.Add(PushToNugetOrg);

Target("default", DependsOn(defaultTargets.ToArray()));

await RunTargetsAndExitAsync(args);