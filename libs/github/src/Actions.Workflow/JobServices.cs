using YamlDotNet.Core.Events;
using YamlDotNet.RepresentationModel;

namespace Logicality.GitHub.Actions.Workflow;

public class JobServices
{
    public readonly IDictionary<string, JobService> _services = new Dictionary<string, JobService>();

    public JobServices(Job job)
    {
        Job = job;
    }

    public Job Job { get; }

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