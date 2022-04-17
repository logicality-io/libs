using YamlDotNet.Core.Events;
using YamlDotNet.RepresentationModel;

namespace Logicality.GitHub.Actions.Workflow;

public class JobContainer
{
    private readonly string           _image;
    private          JobContainerEnv? _env;
    private          int[]?           _ports;
    private          string[]?        _volumes;
    private          string?          _options;
    private          string?          _username;
    private          string?          _password;

    public JobContainer(Job job, string image)
    {
        _image = image;
        Job    = job;
    }

    public JobContainerEnv Env()
    {
        _env = new JobContainerEnv(this);
        return _env;
    }

    public JobContainer Ports(params int[] ports)
    {
        _ports = ports;
        return this;
    }

    public JobContainer Volumes(params string[] volumes)
    {
        _volumes = volumes;
        return this;
    }

    public JobContainer Options(string options)
    {
        _options = options;
        return this;
    }

    public JobContainer Credentials(string username, string password)
    {
        _username = username;
        _password = password;
        return this;
    }

    public Job Job { get; }

    public Workflow Workflow => Job.Workflow;

    internal virtual void Build(YamlMappingNode yamlMappingNode, SequenceStyle sequenceStyle)
    {
        var containerMappingNode = new YamlMappingNode
        {
            { "image", new YamlScalarNode(_image) }
        };

        if (!string.IsNullOrWhiteSpace(_username))
        {
            var credentialsMappingNode = new YamlMappingNode
            {
                { "username", new YamlScalarNode(_username) },
                { "password", new YamlScalarNode(_password) }
            };
            containerMappingNode.Add("credentials", credentialsMappingNode);
        }

        if (_env != null)
        {
            _env.Build(containerMappingNode);
        }

        if (_ports != null)
        {

            YamlNode[] ports             = _ports.Select(p => new YamlScalarNode(p.ToString())).ToArray();
            var        portsSequenceNode = new YamlSequenceNode(ports) { Style = sequenceStyle };
            containerMappingNode.Add("ports", portsSequenceNode);
        }

        if (_volumes != null)
        {
            YamlNode[] volumes             = _volumes.Select(p => new YamlScalarNode(p.ToString())).ToArray();
            var        volumesSequenceNode = new YamlSequenceNode(volumes) { Style = sequenceStyle };

            containerMappingNode.Add("volumes", volumesSequenceNode);
        }

        if (_options != null)
        {
            containerMappingNode.Add("options", new YamlScalarNode(_options));
        }

        yamlMappingNode.Add("container", containerMappingNode);
    }
}