using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.RepresentationModel;

namespace Logicality.GitHub.Actions.Workflow;

/// <summary>
/// Represents a workflow_call trigger.
/// </summary>
public class WorkflowCall : Trigger
{
    private readonly Dictionary<string, WorkflowCallInput>  _inputs  = new();
    private readonly Dictionary<string, WorkflowCallOutput> _outputs = new();
    private readonly Dictionary<string, WorkflowCallSecret> _secrets = new();

    internal WorkflowCall(On @on, Workflow workflow)
        : base("workflow_call", @on, workflow)
    {
    }

    /// <summary>
    /// Add an input to the map of inputs that are passed to the called workflow from the caller workflow with a single item.
    /// See https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#onworkflow_callinputs
    /// </summary>
    /// <param name="id">The input id.</param>
    /// <param name="description">The input description.</param>
    /// <param name="default">The default value.</param>
    /// <param name="required">Is the input required.</param>
    /// <param name="type">The input type.</param>
    /// <returns></returns>
    public WorkflowCall AddInput(string id, string description, string @default, bool required, WorkflowCallType type)
    {
        _inputs.Add(id, new(id, description, @default, required, type));
        return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id">The output Id.</param>
    /// <param name="description">The output description.</param>
    /// <param name="value">The output value.</param>
    /// <returns></returns>
    public WorkflowCall AddOutput(string id, string description, string value)
    {
        _outputs.Add(id, new(id, description, value));
        return this;
    }

    /// <summary>
    /// Add a secret to the map of secrets that can be used in the called workflow.
    /// See https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#onworkflow_callsecrets
    /// </summary>
    /// <param name="id"></param>
    /// <param name="description"></param>
    /// <param name="required"></param>
    /// <returns></returns>
    public WorkflowCall AddSecret(
        string id,
        string description,
        bool   required)
    {
        _secrets.Add(id, new(id, description, required));
        return this;
    }


    internal override void Build(YamlMappingNode parent, SequenceStyle sequenceStyle)
    {
        var workflowCallNode = new YamlMappingNode();

        if (_inputs.Any())
        {
            var inputsNode = new YamlMappingNode();
            foreach (var input in _inputs)
            {
                var inputNode = new YamlMappingNode();
                if (!string.IsNullOrWhiteSpace(input.Value.Description))
                {
                    inputNode.Add("description", new YamlScalarNode(input.Value.Description) { Style = ScalarStyle.SingleQuoted  });
                }
                if (!string.IsNullOrWhiteSpace(input.Value.Default))
                {
                    inputNode.Add("default", new YamlScalarNode(input.Value.Default) { Style = ScalarStyle.SingleQuoted });
                }
                inputNode.Add("required", new YamlScalarNode(input.Value.Required.ToString().ToLower()));
                inputNode.Add("type", new YamlScalarNode(input.Value.Type.ToString().ToLower()));

                inputsNode.Add(input.Value.Id, inputNode);
            }
            workflowCallNode.Add("inputs", inputsNode);
        }

        if (_outputs.Any())
        {
            var outputsNode = new YamlMappingNode();
            foreach (var output in _outputs)
            {
                var outputNode = new YamlMappingNode();
                if (!string.IsNullOrWhiteSpace(output.Value.Description))
                {
                    outputNode.Add("description", new YamlScalarNode(output.Value.Description) { Style = ScalarStyle.SingleQuoted });
                }
                outputNode.Add("value", new YamlScalarNode(output.Value.Value));

                outputsNode.Add(output.Value.Id, outputNode);
            }
            workflowCallNode.Add("outputs", outputsNode);
        }

        if (_secrets.Any())
        {
            var secretsNode = new YamlMappingNode();
            foreach (var secret in _secrets)
            {
                var outputNode = new YamlMappingNode();
                if (!string.IsNullOrWhiteSpace(secret.Value.Description))
                {
                    outputNode.Add("description", new YamlScalarNode(secret.Value.Description) { Style = ScalarStyle.SingleQuoted });
                }
                outputNode.Add("required", new YamlScalarNode(secret.Value.Required.ToString().ToLower()));

                secretsNode.Add(secret.Value.Id, outputNode);
            }
            workflowCallNode.Add("secrets", secretsNode);
        }

        parent.Add("workflow_call", workflowCallNode);
    }

    private record WorkflowCallInput(string Id, string Description, string Default, bool Required, WorkflowCallType Type);

    private record WorkflowCallOutput(string Id, string Description, string Value);
}


/// <summary>
/// Represents a workflow call output.
/// </summary>
/// <param name="Id">The output Id.</param>
/// <param name="Description">The output description.</param>
/// <param name="Value">The output value.</param>
public record WorkflowCallOutput(string Id, string Description, string Value);

/// <summary>
/// Represents a workflow call secret.
/// </summary>
/// <param name="Id">The secret Id.</param>
/// <param name="Description">The secret description.</param>
/// <param name="Required">Is the secret required.</param>
public record WorkflowCallSecret(string Id, string Description, bool Required);