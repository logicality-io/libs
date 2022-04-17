using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Logicality.GitHub.Actions.Workflow;

public class WorkflowDtoTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public WorkflowDtoTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    /*[Fact]
    public void Foo()
    {
        new Workflow("workflow")
            .On.PullRequest()


        var workflowDto = new WorkflowDto
        {
            Name = "Blah",
        };

        workflowDto.On.Add("pull_request", new GitTriggerDto
        {
            Types = new [] { "created", "synchronize" },
            Branches = new [] { "main", "dev" },

        });

        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitDefaults | DefaultValuesHandling.OmitEmptyCollections)
            .Build();
        var yaml = serializer.Serialize(workflowDto);

        _testOutputHelper.WriteLine(yaml);
    }*/
}