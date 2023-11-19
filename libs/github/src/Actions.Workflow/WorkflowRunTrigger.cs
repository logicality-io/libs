using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.RepresentationModel;

namespace Logicality.GitHub.Actions.Workflow;

public class WorkflowRun(On on, Workflow workflow) : Trigger("workflow_run", on, workflow)
{
    private string[]? _branches, _branchesIgnore, _types, _workflows;

    public WorkflowRun Workflows(params string[] workflows)
    {
        _workflows = workflows;
        return this;
    }

    public WorkflowRun Types(params string[] types)
    {
        _types = types;
        return this;
    }

    public WorkflowRun Branches(params string[] branches)
    {
        _branches = branches;
        return this;
    }

    public WorkflowRun BranchesIgnore(params string[] branches)
    {
        _branchesIgnore = branches;
        return this;
    }

    internal override void Build(YamlMappingNode parent, SequenceStyle sequenceStyle)
    {
        var yamlMappingNode = new YamlMappingNode();

        if (_workflows != null && _workflows.Any())
        {
            var sequence = new YamlSequenceNode(_workflows
                .Select(s => new YamlScalarNode(s) { Style = ScalarStyle.SingleQuoted } ))
            {
                Style = sequenceStyle
            };
            yamlMappingNode.Add("workflows", sequence);
        }


        if (_types != null && _types.Any())
        {
            var sequence = new YamlSequenceNode(_types.Select(s => new YamlScalarNode(s)))
            {
                Style = sequenceStyle
            };
            yamlMappingNode.Add("types", sequence);
        }

        if (_branches != null && _branches.Any())
        {
            var sequence = new YamlSequenceNode(_branches.Select(s => new YamlScalarNode(s)))
            {
                Style = sequenceStyle
            };
            yamlMappingNode.Add("branches", sequence);
        }
        if (_branchesIgnore != null && _branchesIgnore.Any())
        {
            var sequence = new YamlSequenceNode(_branchesIgnore.Select(s => new YamlScalarNode(s)))
            {
                Style = sequenceStyle
            };
            yamlMappingNode.Add("branches-ignore", sequence);
        }

        parent.Add(EventName, yamlMappingNode);
    }
}