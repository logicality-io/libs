using YamlDotNet.Core.Events;
using YamlDotNet.RepresentationModel;

namespace Logicality.GitHub.Actions.Workflow;

public class Strategy
{
    private IDictionary<string, string[]> _matrix = new Dictionary<string, string[]>();
    private bool?                         _failFast;
    private int                           _maxParallel;

    public Strategy(Job job)
    {
        Job = job;
    }

    public Job      Job      { get; }
    public Workflow Workflow => Job.Workflow;

    public Strategy Matrix(IDictionary<string, string[]> matrix)
    {
        _matrix = matrix;
        return this;
    }

    public Strategy FailFast(bool failFast)
    {
        _failFast = failFast;
        return this;
    }

    public Strategy MaxParallel(int maxParallel)
    {
        _maxParallel = maxParallel;
        return this;
    }

    public void Build(YamlMappingNode yamlMappingNode, SequenceStyle sequenceStyle)
    {
        var node = new YamlMappingNode();
        if (_matrix.Any())
        {
            var matricsMappingNode = new YamlMappingNode();
            foreach (var matrix in _matrix)
            {
                var values = new YamlSequenceNode { Style = sequenceStyle };
                foreach (var value in matrix.Value)
                {
                    values.Add(value);
                }
                matricsMappingNode.Add(matrix.Key, values);
            }

            node.Add("matrix", matricsMappingNode);
        }

        if (_failFast.HasValue)
        {
            node.Add("fail-fast", _failFast.Value.ToString().ToLower());
        }

        if (_maxParallel != 0)
        {
            node.Add("max-parallel", _maxParallel.ToString());
        }

        yamlMappingNode.Add("strategy", node);
    }
}