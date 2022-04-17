using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.RepresentationModel;

namespace Logicality.GitHub.Actions.Workflow;

public class WorkflowCall : Trigger
{
    private readonly Dictionary<string, InputInfo>  _inputs  = new();
    private readonly Dictionary<string, OutputInfo> _outputs = new();
    private readonly Dictionary<string, SecretInfo> _secrets = new();

    internal WorkflowCall(On @on, Workflow workflow)
        : base("workflow_call", @on, workflow)
    {
    }
       
    public WorkflowCall Input(
        string           id,
        string           description,
        string           @default,
        bool             required,
        WorkflowCallType type)
    {
        _inputs.Add(id, new InputInfo(id, description, @default, required, type));
        return this;
    }

    public WorkflowCall Output(
        string id,
        string description,
        string value)
    {
        _outputs.Add(id, new OutputInfo(id, description, value));
        return this;
    }

    public WorkflowCall Secret(
        string id,
        string description,
        bool   required)
    {
        _secrets.Add(id, new SecretInfo(id, description, required));
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

    private record InputInfo(string Id, string Description, string Default, bool Required, WorkflowCallType Type);

    private record OutputInfo(string Id, string Description, string Value);

    private record SecretInfo(string Id, string Description, bool Required);
}