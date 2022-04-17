using System.Globalization;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.RepresentationModel;

namespace Logicality.GitHub.Actions.Workflow;

public class WorkflowDispatch : Trigger
{
    private WorkflowDispatchInput[]? _inputs;

    public WorkflowDispatch(On on, Workflow workflow)
        : base("workflow_dispatch", on, workflow)
    { }

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

public abstract class WorkflowDispatchInput
{
    protected WorkflowDispatchInput(string id, string description, bool required)
    {
        Description  = description;
        Id           = id;
        Required     = required;
    }

    public             string Id          { get; }
    protected          string Description { get; }
    protected          bool   Required    { get; }
    protected abstract string Type        { get; }

    public virtual YamlMappingNode Build(SequenceStyle sequenceStyle) =>
        new()
        {
            { "description", new YamlScalarNode(Description) { Style = ScalarStyle.SingleQuoted } },
            { "type", Type },
            { "required", Required.ToString().ToLower() },
        };
}

public abstract class WorkflowDispatchInput<T> : WorkflowDispatchInput
{
    protected WorkflowDispatchInput(string id, string description, bool required, T defaultValue)
        : base(id, description, required)
    {
        DefaultValue = defaultValue;
    }

    protected T DefaultValue { get; }
}

public class NumberInput : WorkflowDispatchInput<double>
{
    public NumberInput(string id, string description, bool required, double defaultValue)
        : base(id, description, required, defaultValue)
    { }

    protected override string Type => "booleean";

    public override YamlMappingNode Build(SequenceStyle sequenceStyle)
    {
        var mappingNode = base.Build(sequenceStyle);
        mappingNode.Add("default", DefaultValue.ToString(CultureInfo.InvariantCulture));
        return mappingNode;
    }
}

public class BooleanInput : WorkflowDispatchInput<bool?>
{
    public BooleanInput(string id, string description, bool required, bool? defaultValue = null)
        : base(id, description, required, defaultValue)
    { }

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

public class StringInput : WorkflowDispatchInput<string?>
{
    public StringInput(string id, string description, bool required, string? defaultValue = null)
        : base(id, description, required, defaultValue)
    { }

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

public class ChoiceInput : WorkflowDispatchInput<string?>
{
    private readonly string[] _options;

    public ChoiceInput(string id, string description, bool required, string[] options, string? defaultValue = null)
        : base(id, description, required, defaultValue)
    {
        _options      = options;
    }

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
        foreach (var option in _options)
        {
            sequenceNode.Add(option);
        }
        mappingNode.Add("options", sequenceNode);
        return mappingNode;
    }
}

public class EnvironmentInput : WorkflowDispatchInput
{
    public EnvironmentInput(string id, string description, bool required)
        : base(id, description, required)
    { }

    protected override string Type => "environment";
}
