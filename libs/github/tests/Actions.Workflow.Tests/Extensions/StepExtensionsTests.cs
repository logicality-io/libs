using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logicality.GitHub.Actions.Workflow.Extensions
{
    public class StepExtensions
    {
        [Fact]
        public void ActionsDotnetSetupTests()
        {
            var actual = new Workflow("workflow")
                .Job("build")
                .Step()
                .ActionsSetupDotNet(["8.0.x", "9.0.x"])
                .Workflow
                .GetYaml();

            var expected = Workflow.Header + @"

name: workflow
jobs:
  build:
    steps:
    - name: Setup Dotnet
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: |-
          8.0.x
          9.0.x
";

            actual.ShouldBe(expected.ReplaceLineEndings()); ;
        }
	}
}
