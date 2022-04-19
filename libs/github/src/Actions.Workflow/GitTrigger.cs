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

    /// <summary>
    /// Define the type of activity that will trigger a workflow run.
    /// See https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#onevent_nametypes
    /// </summary>
    /// <param name="types"></param>
    /// <returns></returns>
    public GitTrigger Types(params string[] types)
    {
        _types = types;
        return this;
    }

    /// <summary>
    /// Braches that can trigger a workflow. 
    /// See https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#onpull_requestpull_request_targetbranchesbranches-ignore
    /// </summary>
    /// <param name="branches">A collection of branch patterns.</param>
    /// <returns></returns>
    public GitTrigger Branches(params string[] branches)
    {
        _branches = branches;
        return this;
    }

    /// <summary>
    /// Branches to ignore preventing triggering a workflow.
    /// See https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#onpull_requestpull_request_targetbranchesbranches-ignore
    /// </summary>
    /// <param name="branches">A collection of branch patterns.</param>
    /// <returns></returns>
    public GitTrigger BranchesIgnore(params string[] branches)
    {
        _branchesIgnore = branches;
        return this;
    }

    /// <summary>
    /// Paths that can trigger a workflow.
    /// </summary>
    /// <param name="paths">A collection of path patterns.</param>
    /// <returns></returns>
    public GitTrigger Paths(params string[] paths)
    {
        _paths = paths;
        return this;
    }

    /// <summary>
    /// Paths to ignore preventing triggering a workflow.
    /// </summary>
    /// <param name="paths">A collection of path patterns.</param>
    /// <returns></returns>
    public GitTrigger PathsIgnore(params string[] paths)
    {
        _pathsIgnore = paths;
        return this;
    }

    /// <summary>
    /// Tags that can trigger a workflow.
    /// </summary>
    /// <param name="tags">A collection of tag patterns.</param>
    /// <returns></returns>
    public GitTrigger Tags(params string[] tags)
    {
        _tags = tags;
        return this;
    }

    /// <summary>
    /// Tags to ignore preventing triggering a workflow.
    /// </summary>
    /// <param name="tags">A collection of tag patterns.</param>
    /// <returns></returns>
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