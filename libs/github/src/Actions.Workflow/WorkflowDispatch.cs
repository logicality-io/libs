using System.Globalization;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.RepresentationModel;

namespace Logicality.GitHub.Actions.Workflow;

/// <summary>
/// Represents a workflow_dispatch trigger.
/// </summary>
public class WorkflowDispatch(On on, Workflow workflow) : Trigger("workflow_dispatch", on, workflow)
{
    private WorkflowDispatchInput[]? _inputs;

    /// <summary>
    /// Specify 
    /// </summary>
    /// <param name="inputs"></param>
    /// <returns></returns>
    public WorkflowDispatch Inputs(params WorkflowDispatchInput[] inputs)
    {
        _inputs = inputs;
        return this;
    }

    internal override void Build(YamlMappingNode parent, SequenceStyle sequenceStyle)
    {
        var mappingNode = new YamlMappingNode();
        if (_inputs != null)
        {
            var inputsMappingNode = new YamlMappingNode();
            foreach (var input in _inputs)
            {
                var inputMappingNode = input.Build(sequenceStyle);
                inputsMappingNode.Add(input.Id, inputMappingNode);
            }
            mappingNode.Add("inputs", inputsMappingNode);
        }
        parent.Add(EventName, mappingNode);
    }
}

public abstract class WorkflowDispatchInput(string id, string description, bool required)
{
    public             string Id          { get; } = id;
    protected          string Description { get; } = description;
    protected          bool   Required    { get; } = required;
    protected abstract string Type        { get; }

    public virtual YamlMappingNode Build(SequenceStyle sequenceStyle) =>
        new()
        {
            { "description", new YamlScalarNode(Description) { Style = ScalarStyle.SingleQuoted } },
            { "type", Type },
            { "required", Required.ToString().ToLower() },
        };
}

public abstract class WorkflowDispatchInput<T>(string id, string description, bool required, T defaultValue)
    : WorkflowDispatchInput(id, description, required)
{
    protected T DefaultValue { get; } = defaultValue;
}

public class NumberInput(string id, string description, bool required, double defaultValue)
    : WorkflowDispatchInput<double>(id, description, required, defaultValue)
{
    protected override string Type => "booleean";

    public override YamlMappingNode Build(SequenceStyle sequenceStyle)
    {
        var mappingNode = base.Build(sequenceStyle);
        mappingNode.Add("default", DefaultValue.ToString(CultureInfo.InvariantCulture));
        return mappingNode;
    }
}

public class BooleanInput(string id, string description, bool required, bool? defaultValue = null)
    : WorkflowDispatchInput<bool?>(id, description, required, defaultValue)
{
    protected override string Type => "boolean";

    public override YamlMappingNode Build(SequenceStyle sequenceStyle)
    {
        var mappingNode = base.Build(sequenceStyle);
        if (DefaultValue.HasValue)
        {
            mappingNode.Add("default", DefaultValue.Value.ToString().ToLower());
        }
        return mappingNode;
    }
}

public class StringInput(string id, string description, bool required, string? defaultValue = null)
    : WorkflowDispatchInput<string?>(id, description, required, defaultValue)
{
    protected override string Type => "string";

    public override YamlMappingNode Build(SequenceStyle sequenceStyle)
    {
        var mappingNode = base.Build(sequenceStyle);
        if (DefaultValue != null)
        {
            mappingNode.Add("default", new YamlScalarNode(DefaultValue) { Style = ScalarStyle.SingleQuoted });
        }
        return mappingNode;
    }
}

public class ChoiceInput(string id, string description, bool required, string[] options, string? defaultValue = null)
    : WorkflowDispatchInput<string?>(id, description, required, defaultValue)
{
    protected override string Type => "choice";

    public override YamlMappingNode Build(SequenceStyle sequenceStyle)
    {
        var mappingNode = base.Build(sequenceStyle);
        if (DefaultValue != null)
        {
            mappingNode.Add("default", new YamlScalarNode(DefaultValue) { Style = ScalarStyle.SingleQuoted });
        }
        var sequenceNode = new YamlSequenceNode
        {
            Style = sequenceStyle
        };
        foreach (var option in options)
        {
            sequenceNode.Add(option);
        }
        mappingNode.Add("options", sequenceNode);
        return mappingNode;
    }
}

public class EnvironmentInput(string id, string description, bool required)
    : WorkflowDispatchInput(id, description, required)
{
    protected override string Type => "environment";
}
