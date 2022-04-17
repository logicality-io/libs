using System.Collections.Generic;
using Shouldly;
using Xunit;
using YamlDotNet.Core.Events;

namespace Logicality.GitHub.Actions.Workflow;

public class JobTests
{
    [Fact]
    public void Job_Name()
    {
        var actual = new Workflow("workflow")
            .Job("build")
            .Name("Build Project")
            .Workflow
            .GetYaml();

        var expected = Workflow.Header + @"

name: workflow
jobs:
  build:
    name: Build Project
";

        actual.ShouldBe(expected);
    }

    [Fact]
    public void Job_Needs()
    {
        var actual = new Workflow("workflow")
            .Job("job3")
            .Needs("job1", "job2")
            .Workflow
            .GetYaml(SequenceStyle.Flow);

        var expected = Workflow.Header + @"

name: workflow
jobs:
  job3:
    needs: [job1, job2]
";

        actual.ShouldBe(expected);
    }

    [Fact]
    public void Job_RunsOn()
    {
        var actual = new Workflow("workflow")
            .Job("build")
            .RunsOn("ubuntu-latest")
            .Workflow
            .GetYaml();

        var expected = Workflow.Header + @"

name: workflow
jobs:
  build:
    runs-on: ubuntu-latest
";

        actual.ShouldBe(expected);
    }

    [Fact]
    public void Job_Permissions()
    {
        var actual = new Workflow("workflow")
            .Job("build")
            .Permissions(packages: Permission.Write)
            .Workflow
            .GetYaml();

        var expected = Workflow.Header + @"

name: workflow
jobs:
  build:
    permissions:
      packages: write
";

        actual.ShouldBe(expected);
    }

    [Fact]
    public void Job_PermissionsReadAll()
    {
        var actual = new Workflow("workflow")
            .Job("build")
            .PermissionsReadAll()
            .Workflow
            .GetYaml();

        var expected = Workflow.Header + @"

name: workflow
jobs:
  build:
    permissions: read-all
";

        actual.ShouldBe(expected);
    }

    [Fact]
    public void Job_PermissionsWriteAll()
    {
        var actual = new Workflow("workflow")
            .Job("build")
            .PermissionsWriteAll()
            .Workflow
            .GetYaml();

        var expected = Workflow.Header + @"

name: workflow
jobs:
  build:
    permissions: write-all
";

        actual.ShouldBe(expected);
    }

    [Fact]
    public void Job_Environment()
    {
        var actual = new Workflow("workflow")
            .Job("build")
            .Environment("prod", "http://example.com")
            .Workflow
            .GetYaml();

        var expected = Workflow.Header + @"

name: workflow
jobs:
  build:
    environment:
      name: prod
      url: http://example.com
";

        actual.ShouldBe(expected);
    }

    [Fact]
    public void Job_Concurrency()
    {
        var actual = new Workflow("workflow")
            .Job("build")
            .Concurrency("${{ github.head_ref }}", true)
            .Workflow
            .GetYaml();

        var expected = Workflow.Header + @"

name: workflow
jobs:
  build:
    concurrency:
      group: ${{ github.head_ref }}
      cancel-in-progress: true
";

        actual.ShouldBe(expected);
    }

    [Fact]
    public void Job_Outputs()
    {
        var actual = new Workflow("workflow")
            .Job("build")
            .Outputs(new Dictionary<string, string>
            {
                { "output1", "${{ steps.step1.outputs.test }}" },
                { "output2", "${{ steps.step2.outputs.test }}" }
            })
            .Workflow
            .GetYaml();

        var expected = Workflow.Header + @"

name: workflow
jobs:
  build:
    outputs:
      output1: ${{ steps.step1.outputs.test }}
      output2: ${{ steps.step2.outputs.test }}
";

        actual.ShouldBe(expected);
    }

    [Fact]
    public void Job_Env()
    {
        var actual = new Workflow("workflow")
            .Job("build")
            .Env(new Dictionary<string, string>
            {
                { "GITHUB_TOKEN", "${{secrets.GITHUB_TOKEN}}" }
            })
            .Workflow
            .GetYaml();

        var expected = Workflow.Header + @"

name: workflow
jobs:
  build:
    env:
      GITHUB_TOKEN: ${{secrets.GITHUB_TOKEN}}
";

        actual.ShouldBe(expected);
    }

    [Fact]
    public void Job_Defaults()
    {
        var actual = new Workflow("workflow")
            .Job("build")
            .Defaults(new Dictionary<string, string>
            {
                { "foo", "bar" }
            })
            .Workflow
            .GetYaml();

        var expected = Workflow.Header + @"

name: workflow
jobs:
  build:
    defaults:
      foo: bar
";

        actual.ShouldBe(expected);
    }

    [Fact]
    public void Job_Defaults_Run()
    {
        var actual = new Workflow("workflow")
            .Job("build")
            .DefaultsRun("bash", "./dir")
            .Workflow
            .GetYaml();

        var expected = Workflow.Header + @"

name: workflow
jobs:
  build:
    defaults:
      run:
        shell: bash
        working-directory: ./dir
";

        actual.ShouldBe(expected);
    }

    [Fact]
    public void Job_If()
    {
        var actual = new Workflow("workflow")
            .Job("build")
            .If("${{ <expression> }}")
            .Workflow
            .GetYaml();

        var expected = Workflow.Header + @"

name: workflow
jobs:
  build:
    if: ${{ <expression> }}
";

        actual.ShouldBe(expected);
    }

    [Fact]
    public void Job_TimeoutMinutes()
    {
        var actual = new Workflow("workflow")
            .Job("build")
            .TimeoutMinutes(10)
            .Workflow
            .GetYaml();

        var expected = Workflow.Header + @"

name: workflow
jobs:
  build:
    timeout-minutes: 10
";

        actual.ShouldBe(expected);
    }

    [Fact]
    public void Job_Strategy_Matrix()
    {
        var actual = new Workflow("workflow")
            .Job("build")
            .Strategy()
            .Matrix(new Dictionary<string, string[]>
            {
                {"os", new [] { "ubuntu-18.04", "ubuntu-20.04"} },
                {"node", new [] { "10", "12", "14"} },
            })
            .Workflow
            .GetYaml(SequenceStyle.Flow);

        var expected = Workflow.Header + @"

name: workflow
jobs:
  build:
    strategy:
      matrix:
        os: [ubuntu-18.04, ubuntu-20.04]
        node: [10, 12, 14]
";

        actual.ShouldBe(expected);
    }

    [Fact]
    public void Job_Strategy_FailFast()
    {
        var actual = new Workflow("workflow")
            .Job("build")
            .Strategy()
            .FailFast(false)
            .Workflow
            .GetYaml();

        var expected = Workflow.Header + @"

name: workflow
jobs:
  build:
    strategy:
      fail-fast: false
";

        actual.ShouldBe(expected);
    }

    [Fact]
    public void Job_Strategy_MaxParallel()
    {
        var actual = new Workflow("workflow")
            .Job("build")
            .Strategy()
            .MaxParallel(2)
            .Workflow
            .GetYaml();

        var expected = Workflow.Header + @"

name: workflow
jobs:
  build:
    strategy:
      max-parallel: 2
";

        actual.ShouldBe(expected);
    }

    [Fact]
    public void Job_ContinueOnError()
    {
        var actual = new Workflow("workflow")
            .Job("build")
            .ContinueOnError(true)
            .Workflow
            .GetYaml();

        var expected = Workflow.Header + @"

name: workflow
jobs:
  build:
    continue-on-error: true
";

        actual.ShouldBe(expected);
    }

    [Fact]
    public void Job_Uses()
    {
        var actual = new Workflow("workflow")
            .Job("build")
            .Uses("octo-org/this-repo/.github/workflows/workflow-1.yml@172239021f7ba04fe7327647b213799853a9eb89")
            .Workflow
            .GetYaml();

        var expected = Workflow.Header + @"

name: workflow
jobs:
  build:
    uses: octo-org/this-repo/.github/workflows/workflow-1.yml@172239021f7ba04fe7327647b213799853a9eb89
";

        actual.ShouldBe(expected);
    }

    [Fact]
    public void Job_With()
    {
        var actual = new Workflow("workflow")
            .Job("build")
            .Uses("octo-org/this-repo/.github/workflows/workflow-1.yml@172239021f7ba04fe7327647b213799853a9eb89")
            .With()
                .Key("foo", "bar")
            .Workflow
            .GetYaml();

        var expected = Workflow.Header + @"

name: workflow
jobs:
  build:
    uses: octo-org/this-repo/.github/workflows/workflow-1.yml@172239021f7ba04fe7327647b213799853a9eb89
    with:
      foo: bar
";

        actual.ShouldBe(expected);
    }
}