using Shouldly;
using Xunit;

namespace Logicality.GitHub.Actions.Workflow;

public class JobStepTests
{
    [Fact]
    public void Step_Name()
    {
        var actual = new Workflow("workflow")
            .Job("build")
            .Step("step")
            .Name("The Step")
            .Workflow
            .GetYaml();

        var expected = Workflow.Header + @"

name: workflow
jobs:
  build:
    steps:
    - id: step
      name: The Step
";
      
        actual.ShouldBe(expected);
    }

    [Fact]
    public void Step_If()
    {
        var actual = new Workflow("workflow")
            .Job("build")
            .Step("step")
            .If("${{<expression>}}")
            .Workflow
            .GetYaml();

        var expected = Workflow.Header + @"

name: workflow
jobs:
  build:
    steps:
    - id: step
      if: ${{<expression>}}
";

        actual.ShouldBe(expected);
    }

    [Fact]
    public void Step_Run()
    {
        var actual = new Workflow("workflow")
            .Job("build")
            .Step("step")
            .Run("./build.ps1 push")
            .Workflow
            .GetYaml();

        var expected = Workflow.Header + @"

name: workflow
jobs:
  build:
    steps:
    - id: step
      run: ./build.ps1 push
";

        actual.ShouldBe(expected);
    }

    [Fact]
    public void Step_Uses()
    {
        var actual = new Workflow("workflow")
            .Job("build")
            .Step("step")
            .Uses("action/action:v1.0")
            .Workflow
            .GetYaml();

        var expected = Workflow.Header + @"

name: workflow
jobs:
  build:
    steps:
    - id: step
      uses: action/action:v1.0
";

        actual.ShouldBe(expected);
    }

    [Fact]
    public void Step_Shell()
    {
        var actual = new Workflow("workflow")
            .Job("build")
            .Step("step")
            .Shell("bash")
            .Workflow
            .GetYaml();

        var expected = Workflow.Header + @"

name: workflow
jobs:
  build:
    steps:
    - id: step
      shell: bash
";

        actual.ShouldBe(expected);
    }

    [Fact]
    public void Step_With()
    {
        var actual = new Workflow("workflow")
            .Job("build")
            .Step("step")
            .Uses("action/action")
            .With("foo", "bar")
            .Workflow
            .GetYaml();

        var expected = Workflow.Header + @"

name: workflow
jobs:
  build:
    steps:
    - id: step
      uses: action/action
      with:
        foo: bar
";

        actual.ShouldBe(expected);
    }

    [Fact]
    public void Step_TimeoutMinutes()
    {
        var actual = new Workflow("workflow")
            .Job("build")
            .Step("step")
            .TimeoutMinutes(10)
            .Workflow
            .GetYaml();

        var expected = Workflow.Header + @"

name: workflow
jobs:
  build:
    steps:
    - id: step
      timeout-minutes: 10
";

        actual.ShouldBe(expected);
    }

    [Fact]
    public void Step_ContinueOnError()
    {
        var actual = new Workflow("workflow")
            .Job("build")
            .Step("step")
            .ContinueOnError(true)
            .Workflow
            .GetYaml();

        var expected = Workflow.Header + @"

name: workflow
jobs:
  build:
    steps:
    - id: step
      continue-on-error: true
";

        actual.ShouldBe(expected);
    }
}