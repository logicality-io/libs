using YamlDotNet.RepresentationModel;

namespace Logicality.GitHub.Actions.Workflow;

public class JobDefaults : JobKeyValueMap<JobDefaults>
{
    private string? _shell;
    private string? _workingDirectory;

    public JobDefaults(Job job) : base(job, "defaults")
    {
    }

    public JobDefaults(Job job, IDictionary<string, string> map)
        : base(job, "defaults", map)
    {
    }

    /// <summary>
    /// Set default shell and working-directory to all run steps in the job.
    /// https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#jobsjob_iddefaultsrun
    /// </summary>
    /// <param name="shell"></param>
    /// <param name="workingDirectory"></param>
    /// <returns></returns>
    public JobDefaults Run(string shell, string workingDirectory)
    {
        _shell         = shell;
        _workingDirectory = workingDirectory;
        return this;
    }

    internal override void Build(YamlMappingNode yamlMappingNode)
    {
        if (Map.Any() || !string.IsNullOrWhiteSpace(_shell))
        {
            var defaultsMappingNode = new YamlMappingNode();
            foreach (var property in Map)
            {
                defaultsMappingNode.Add(property.Key, new YamlScalarNode(property.Value));
            }
            // Defaults Run
            if (!string.IsNullOrWhiteSpace(_shell) && !string.IsNullOrWhiteSpace(_workingDirectory))
            {
                var defaultsRunMappingNode = new YamlMappingNode
                {
                    { "shell", _shell },
                    { "working-directory", _workingDirectory }
                };
                defaultsMappingNode.Add("run", defaultsRunMappingNode);
            }
            yamlMappingNode.Add("defaults", defaultsMappingNode);
        }
    }
}