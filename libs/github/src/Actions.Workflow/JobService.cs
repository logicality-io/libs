using YamlDotNet.Core.Events;
using YamlDotNet.RepresentationModel;

namespace Logicality.GitHub.Actions.Workflow;

public class JobService
{
    private readonly string         _id;
    private readonly string         _image;
    private          JobServiceEnv? _env;
    private          int[]?         _ports;
    private          string[]?      _volumes;
    private          string?        _options;
    private          string?        _username;
    private          string?        _password;

    public JobService(JobServices jobServices, string id, string image)
    {
        _id = id;
        _image   = image;
        Services = jobServices;
    }

    public JobServices Services { get; }

    public Job Job => Services.Job;

    public Workflow Workflow => Services.Job.Workflow;

    public JobServiceEnv Env()
    {
        _env = new JobServiceEnv(this);
        return _env;
    }

    public JobService Ports(params int[] ports)
    {
        _ports = ports;
        return this;
    }

    public JobService Volumes(params string[] volumes)
    {
        _volumes = volumes;
        return this;
    }

    public JobService Options(string options)
    {
        _options = options;
        return this;
    }

    public JobService Credentials(string username, string password)
    {
        _username = username;
        _password = password;
        return this;
    }

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

        yamlMappingNode.Add(_id, containerMappingNode);
    }
}