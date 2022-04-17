# GitHub Actions Workflow Generator

## Introduction

A library to help generate GitHub Actions Workflows yaml in a (reasonably)
strong typed manner. Written to support a better authoring experience, increased
reusability and mitigate against the fragility of copy'n'pasting stuff around.
If one is maintain a few workflows, this is probably over-kill. If one is
maintaining many workflows across many repositories, this may be useful.

## Quick Example

The following code:

```csharp

var workflow = new Workflow("my-workflow");

workflow.On
    .Push()
    .Branches("main")
    
var buildJob = workflow
    .Job("build")
    .RunsOn(GitHubHostedRunner.UbuntuLatest);

buildJob.StepActionsCheckout();

buildJob.Step()
    .Name("Build")
    .Run($"./build.ps1")
    .ShellPowerShell();

var yaml = workflow.GetYaml();

var fileName = "my-workflow.yaml

var yaml     = workflow.GetYaml();
var filePath = "../workflows/my-workflow.yml";

File.WriteAllText(filePath, yaml);
```

... will generate the following yaml:

```yaml
name: my-workflow
on:
  push:
    branches:
    - main
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
    - name: Checkout
      uses: actions/checkout@v3
      with:
        fetch-depth: 0
    - name: Build
      run: ./build.ps1
      shell: pwsh
```

## Using
