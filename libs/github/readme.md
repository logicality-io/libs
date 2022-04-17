# GitHub Actions Workflow Generator

## Introduction

A library to help generate GitHub Actions Workflows yaml in a (reasonably)
strong typed manner. Written to support a better authoring experience, increased
reusability and mitigate against the fragility of copy'n'pasting stuff around.
If one is maintain a few workflows, this is probably over-kill. If one is
maintaining many workflows across many repositories, this may be useful.

## Example

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

workflow.WriteYaml("../workflows/my-workflow.yml", yaml);
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

1. Create a new console project `WorkflowGen` (or what ever you want call it). I
   recommend putting in the ./github/ directory.

   `dotnet new console --name WorkflowGen`

2. Add the necessary package reference:

   ```powershell
   cd WorkflowGen
   dotnet add package Logicality.GitHub.Actions.Workflow --prerelease
   ```

3. Add some workflow generating code:

   ```csharp
   using Logicality.GitHub.Actions.Workflow;

   var workflow = new Workflow("my-workflow");

    workflow.On
      .Push()
      .Branches("main");

    // Workflow path relative to console project
    workflow.WriteYaml("../workflows/my-workflow.yaml"); 

   ```

4. Generate the workflow:

    `dotnet run`

5. Re-generate the workflow when ever you make changes by calling `dotnet run`

See [WorkflowGen](../../.github/WorkflowGen/Program.cs) for some inspiration.
The API _should_ be discoverable if one is familiar with [GitHub Workflow
Syntax](https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions). This project is not a substitute for understanding and learning the syntax.

## Contributing

- Open a discussion if any questions, feedback or requests.
- Open an issue if there are any bugs with a full reproducible.