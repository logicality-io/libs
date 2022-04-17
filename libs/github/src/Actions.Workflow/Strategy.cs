using YamlDotNet.Core.Events;
using YamlDotNet.RepresentationModel;

namespace Logicality.GitHub.Actions.Workflow;

public class Strategy
{
    private Matrix? _matrix;
    private bool?   _failFast;
    private int     _maxParallel;

    public Strategy(Job job)
    {
        Job = job;
    }

    public Job      Job      { get; }
    public Workflow Workflow => Job.Workflow;

    public Matrix Matrix()
    {
        _matrix = new Matrix(this);
        return _matrix;
    }

    public Matrix Matrix(IDictionary<string, string[]> properties)
    {
        _matrix = new Matrix(this, properties);
        return _matrix;
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

    internal void Build(YamlMappingNode yamlMappingNode, SequenceStyle sequenceStyle)
    {
        var node = new YamlMappingNode();

        if (_matrix != null)
        {
            _matrix.Build(node, sequenceStyle);
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