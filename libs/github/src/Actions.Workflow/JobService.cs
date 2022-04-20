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

    internal JobService(JobServices jobServices, string id, string image)
    {
        _id = id;
        _image   = image;
        Services = jobServices;
    }

    public JobServices Services { get; }

    public Job Job => Services.Job;

    public Workflow Workflow => Services.Job.Workflow;

    /// <summary>
    /// Sets a map of environment variables in the service container.
    /// See https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#jobsjob_idservicesservice_idenv
    /// </summary>
    /// <param name="map"></param>
    /// <returns></returns>
    public JobService Env(params (string Key, string Value)[] map)
    {
        _env = new(this, map.ToDictionary());
        return _env.Service;
    }

    /// <summary>
    /// Sets an array of ports to expose on the service container.
    /// See https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#jobsjob_idservicesservice_idports
    /// </summary>
    /// <param name="ports"></param>
    /// <returns></returns>
    public JobService Ports(params int[] ports)
    {
        _ports = ports;
        return this;
    }

    /// <summary>
    /// Sets an array of volumes for the service container to use. 
    /// See https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#jobsjob_idservicesservice_idvolumes
    /// </summary>
    /// <param name="volumes"></param>
    /// <returns></returns>
    public JobService Volumes(params string[] volumes)
    {
        _volumes = volumes;
        return this;
    }

    /// <summary>
    /// Configure additional Docker container resource options.
    /// See https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#jobsjob_idcontaineroptions
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public JobService Options(string options)
    {
        _options = options;
        return this;
    }

    /// <summary>
    /// Credentials for the container registry to pull the image,
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public JobService Credentials(string username, string password)
    {
        _username = username;
        _password = password;
        return this;
    }

    internal void Build(YamlMappingNode yamlMappingNode, SequenceStyle sequenceStyle)
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