using YamlDotNet.Core.Events;
using YamlDotNet.RepresentationModel;

namespace Logicality.GitHub.Actions.Workflow;

public class GitTrigger : Trigger
{
    private string[]? _types, _branches, _branchesIgnore, _paths, _pathsIgnore, _tags, _tagsIgnore;

    internal GitTrigger(string eventName, On @on, Workflow workflow)
        : base(eventName, @on, workflow)
    {
    }

    public GitTrigger Types(params string[] types)
    {
        _types = types;
        return this;
    }

    public GitTrigger Branches(params string[] branches)
    {
        _branches = branches;
        return this;
    }

    public GitTrigger BranchesIgnore(params string[] branches)
    {
        _branchesIgnore = branches;
        return this;
    }

    public GitTrigger Paths(params string[] paths)
    {
        _paths = paths;
        return this;
    }

    public GitTrigger PathsIgnore(params string[] paths)
    {
        _pathsIgnore = paths;
        return this;
    }

    public GitTrigger Tags(params string[] tags)
    {
        _tags = tags;
        return this;
    }

    public GitTrigger TagsIgnore(params string[] tags)
    {
        _tagsIgnore = tags;
        return this;
    }

    internal override void Build(YamlMappingNode parent, SequenceStyle sequenceStyle)
    {
        var yamlMappingNode = new YamlMappingNode();

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

        if (_paths != null && _paths.Any())
        {
            var sequence = new YamlSequenceNode(_paths.Select(s => new YamlScalarNode(s)))
            {
                Style = sequenceStyle
            };
            yamlMappingNode.Add("paths", sequence);
        }

        if (_pathsIgnore != null && _pathsIgnore.Any())
        {
            var sequence = new YamlSequenceNode(_pathsIgnore.Select(s => new YamlScalarNode(s)))
            {
                Style = sequenceStyle
            };
            yamlMappingNode.Add("paths-ignore", sequence);
        }

        if (_tags != null && _tags.Any())
        {
            var sequence = new YamlSequenceNode(_tags.Select(s => new YamlScalarNode(s)))
            {
                Style = sequenceStyle
            };
            yamlMappingNode.Add("tags", sequence);
        }

        if (_tagsIgnore != null && _tagsIgnore.Any())
        {
            var sequence = new YamlSequenceNode(_tagsIgnore.Select(s => new YamlScalarNode(s)))
            {
                Style = sequenceStyle
            };
            yamlMappingNode.Add("tags-ignore", sequence);
        }
        
        parent.Add(EventName, yamlMappingNode);
    }
}