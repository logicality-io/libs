// See https://aka.ms/new-console-template for more information

using System.Text;
using Nuke.Common.CI.GitHubActions.Configuration;
using Nuke.Common.Utilities;

var libs = new [] { "aspnet-core", "bullseye", "configuration", "hosting", "lambda", "pulumi", "system-extensions", "testing"};

var path = "../workflows";

foreach (var lib in libs)
{
    Console.WriteLine($"Genering workflow for {lib}");
    var file = Path.Combine(path, $"{lib}-ci-2.yaml");

    var configuration = new GitHubActionsConfiguration
    {
        Name = $"{lib}-ci",
    };
    using var fileStream   = File.OpenWrite(file);
    using var streamWriter = new StreamWriter(fileStream, Encoding.UTF8);
    var       fileWriter   = new CustomFileWriter(streamWriter, 2, "#");

    configuration.Write(fileWriter);
}