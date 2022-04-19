using YamlDotNet.Core.Events;
using YamlDotNet.RepresentationModel;

namespace Logicality.GitHub.Actions.Workflow;

public class JobServices
{
    private readonly IDictionary<string, JobService> _services = new Dictionary<string, JobService>();

    public JobServices(Job job)
    {
        Job = job;
    }

    public Job Job { get; }

    /// <summary>
    /// Used to host service containers for a job in a workflow.
    /// See https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#jobsjob_idservices
    /// </summary>
    /// <param name="id">The service identifier.</param>
    /// <param name="image">The Docker image to use as the service container to run the action. </param>
    /// <returns></returns>
    public JobService Service(string id, string image)
    {
        var service = new JobService(this, id, image);
        _services.Add(id, service);
        return service;
    }

    internal void Build(YamlMappingNode mappingNode, SequenceStyle sequenceStyle)
    {
        if (_services.Any())
        {
            var servicesNode = new YamlMappingNode();
            foreach (var jobService in _services)
            {
                jobService.Value.Build(servicesNode, sequenceStyle);
            }

            mappingNode.Add("services", servicesNode);
        }
    }
}