using System.Collections.Generic;
using Shouldly;
using Xunit;
using YamlDotNet.Core.Events;

namespace Logicality.GitHub.Actions.Workflow;

public class WorkflowTests
{
    [Fact]
    public void Workflow_On_PullRequest()
    {
        var actual = new Workflow("workflow")
            .On
            .PullRequest()
            .Types("opened", "synchronize")
            .Branches("main", "dev")
            .BranchesIgnore("feature/")
            .Paths("libs/foo**", "build/**")
            .PathsIgnore("docs/")
            .Tags("foo-**")
            .TagsIgnore("release/**")
            .Workflow
            .GetYaml();

        var expected = Workflow.Header + @"

name: workflow
on:
  pull_request:
    types:
    - opened
    - synchronize
    branches:
    - main
    - dev
    branches-ignore:
    - feature/
    paths:
    - libs/foo**
    - build/**
    paths-ignore:
    - docs/
    tags:
    - foo-**
    tags-ignore:
    - release/**
";

        actual.ShouldBe(expected.ReplaceLineEndings());;
    }

    [Fact]
    public void Workflow_On_PullRequestTarget()
    {
        var actual = new Workflow("workflow")
            .On
            .PullRequestTarget()
            .Workflow
            .GetYaml();

        var expected = Workflow.Header + @"

name: workflow
on:
  pull_request_target:
";

        actual.ShouldBe(expected.ReplaceLineEndings());;
    }

    [Fact]
    public void Workflow_On_Push()
    {
        var actual = new Workflow("workflow")
            .On
            .Push()
            .Branches("main", "dev")
            .BranchesIgnore("feature/")
            .Paths("libs/foo**", "build/**")
            .PathsIgnore("docs/")
            .Tags("foo-**")
            .TagsIgnore("release/**")
            .Workflow
            .GetYaml();

        var expected = Workflow.Header + @"

name: workflow
on:
  push:
    branches:
    - main
    - dev
    branches-ignore:
    - feature/
    paths:
    - libs/foo**
    - build/**
    paths-ignore:
    - docs/
    tags:
    - foo-**
    tags-ignore:
    - release/**
";

        actual.ShouldBe(expected.ReplaceLineEndings());;
    }

    [Fact]
    public void Workflow_On_Schedule()
    {
        var actual = new Workflow("workflow")
            .On
            .Schedule("0 5,17 * * *", "1 2,3 * * *")
            .Workflow
            .GetYaml();

        var expected = Workflow.Header + @"

name: workflow
on:
  schedule:
  - cron: '0 5,17 * * *'
  - cron: '1 2,3 * * *'
";

        actual.ShouldBe(expected.ReplaceLineEndings());;
    }

    [Fact]
    public void Workflow_On_Event()
    {
        var actual = new Workflow("workflow")
            .On
            .Event("release")
                .Types("published", "created", "edited")
                .Workflow
            .GetYaml();

        var expected = Workflow.Header + @"

name: workflow
on:
  release:
    types:
    - published
    - created
    - edited
";

        actual.ShouldBe(expected.ReplaceLineEndings());;
    }

    [Fact]
    public void Workflow_On_WorkflowCall()
    {
        var actual = new Workflow("workflow")
            .On
            .WorkflowCall()
            .AddInput("username", "A username passed from the caller workflow", "john-doe", false, WorkflowCallType.String)
            .AddInput("path", "A path passed from the caller workflow", "./", true, WorkflowCallType.String)
            .AddOutput("workflow_output1", "The first job output", "${{ jobs.my_job.outputs.job_output1 }}")
            .AddSecret("access-token", "A token passed from the caller workflow", false)
            .Workflow
            .GetYaml();

        var expected = Workflow.Header + @"

name: workflow
on:
  workflow_call:
    inputs:
      username:
        description: 'A username passed from the caller workflow'
        default: 'john-doe'
        required: false
        type: string
      path:
        description: 'A path passed from the caller workflow'
        default: './'
        required: true
        type: string
    outputs:
      workflow_output1:
        description: 'The first job output'
        value: ${{ jobs.my_job.outputs.job_output1 }}
    secrets:
      access-token:
        description: 'A token passed from the caller workflow'
        required: false
";

        actual.ShouldBe(expected.ReplaceLineEndings());;
    }

    [Fact]
    public void Workflow_On_WorkflowRun()
    {
        var actual = new Workflow("workflow")
            .On
            .WorkflowRun()
            .Workflows("foo", "bar")
            .Branches("main")
            .Workflow
            .GetYaml(SequenceStyle.Flow);

        var expected = Workflow.Header + @"

name: workflow
on:
  workflow_run:
    workflows: ['foo', 'bar']
    branches: [main]
";

        actual.ShouldBe(expected.ReplaceLineEndings());;
    }


    [Fact]
    public void Workflow_On_WorkflowDispatch()
    {
        var actual = new Workflow("workflow")
            .On
            .WorkflowDispatch()
            .Inputs(
                new ChoiceInput("logLevel", "Log level", true, new []{ "info", "warning", "debug"}, "warning"),
                new BooleanInput("print_tags", "True to print to STDOUT", true),
                new StringInput("tags", "Test scenario tags", true),
                new EnvironmentInput("environment", "Environment to run tests against", true))
            .Workflow
            .GetYaml();

        var expected = Workflow.Header + @"

name: workflow
on:
  workflow_dispatch:
    inputs:
      logLevel:
        description: 'Log level'
        type: choice
        required: true
        default: 'warning'
        options:
        - info
        - warning
        - debug
      print_tags:
        description: 'True to print to STDOUT'
        type: boolean
        required: true
      tags:
        description: 'Test scenario tags'
        type: string
        required: true
      environment:
        description: 'Environment to run tests against'
        type: environment
        required: true
";

        actual.ShouldBe(expected.ReplaceLineEndings());;
    }

    [Fact]
    public void Workflow_Permissions()
    {
        var actual = new Workflow("workflow")
            .Permissions(packages: Permission.Write)
            .GetYaml();

        var expected = Workflow.Header + @"

name: workflow
permissions:
  packages: write
";

        actual.ShouldBe(expected.ReplaceLineEndings());;
    }

    [Fact]
    public void Workflow_PermissionsReadAll()
    {
        var actual = new Workflow("workflow")
            .PermissionsReadAll()
            .GetYaml();

        var expected = Workflow.Header + @"

name: workflow
permissions: read-all
";

        actual.ShouldBe(expected.ReplaceLineEndings());;
    }

    [Fact]
    public void Workflow_PermissionsWriteAll()
    {
        var actual = new Workflow("workflow")
            .PermissionsWriteAll()
            .GetYaml();

        var expected = Workflow.Header + @"

name: workflow
permissions: write-all
";

        actual.ShouldBe(expected.ReplaceLineEndings());;
    }

    [Fact]
    public void Workflow_Concurrency()
    {
        var actual = new Workflow("workflow")
            .Concurrency("${{ github.head_ref }}", true)
            .GetYaml();

        var expected = Workflow.Header + @"

name: workflow
concurrency:
  group: ${{ github.head_ref }}
  cancel-in-progress: true
";

        actual.ShouldBe(expected.ReplaceLineEndings());;
    }

    [Fact]
    public void Workflow_Env()
    {
        var actual = new Workflow("workflow")
            .Env(("GITHUB_TOKEN", "${{secrets.GITHUB_TOKEN}}"))
            .GetYaml();

        var expected = Workflow.Header + @"

name: workflow
env:
  GITHUB_TOKEN: ${{secrets.GITHUB_TOKEN}}
";

        actual.ShouldBe(expected.ReplaceLineEndings());;
    }


    [Fact]
    public void Workflow_Defaults()
    {
        var actual = new Workflow("workflow")
            .Defaults()
            .Key("foo", "bar")
            .Workflow
            .GetYaml();

        var expected = Workflow.Header + @"

name: workflow
defaults:
  foo: bar
";

        actual.ShouldBe(expected.ReplaceLineEndings());;
    }

    [Fact]
    public void Workflow_Defaults_Run()
    {
        var actual = new Workflow("workflow")
            .Defaults()
            .Run("bash", "./dir")
            .Workflow
            .GetYaml();

        var expected = Workflow.Header + @"

name: workflow
defaults:
  run:
    shell: bash
    working-directory: ./dir
";

        actual.ShouldBe(expected.ReplaceLineEndings());;
    }
}
