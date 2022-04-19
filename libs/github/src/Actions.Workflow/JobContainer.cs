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

    /// <summary>
    /// Sets a map of environment variables in the container.
    /// See https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#jobsjob_idcontainerenv
    /// </summary>
    /// <param name="map"></param>
    /// <returns></returns>
    public JobContainer Env(IDictionary<string, string> map)
    {
        _env = new(this, map);
        return _env.JobContainer;
    }

    /// <summary>
    /// Sets an array of ports to expose on the container.
    /// See https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#jobsjob_idcontainerports
    /// </summary>
    /// <param name="ports"></param>
    /// <returns></returns>
    public JobContainer Ports(params int[] ports)
    {
        _ports = ports;
        return this;
    }

    /// <summary>
    /// Sets an array of volumes for the container to use. 
    /// See https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#jobsjob_idcontainervolumes
    /// </summary>
    /// <param name="volumes"></param>
    /// <returns></returns>
    public JobContainer Volumes(params string[] volumes)
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
    public JobContainer Options(string options)
    {
        _options = options;
        return this;
    }

    /// <summary>
    /// Credentials for the container registry to pull the image,
    /// See https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#jobsjob_idcontainercredentials
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public JobContainer Credentials(string username, string password)
    {
        _username = username;
        _password = password;
        return this;
    }

    public Job Job { get; }

    public Workflow Workflow => Job.Workflow;

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

        yamlMappingNode.Add("container", containerMappingNode);
    }
}