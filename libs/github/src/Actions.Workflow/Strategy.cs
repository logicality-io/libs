using YamlDotNet.Core.Events;
using YamlDotNet.RepresentationModel;

namespace Logicality.GitHub.Actions.Workflow;

public class Strategy(Job job)
{
    private Matrix? _matrix;
    private bool?   _failFast;
    private int     _maxParallel;

    public Job      Job      { get; } = job;
    public Workflow Workflow => Job.Workflow;

    /// <summary>
    /// Define a matrix of different job configuration.
    /// See https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#jobsjob_idstrategymatrix
    /// </summary>
    /// <param name="map"></param>
    /// <returns></returns>
    public Strategy Matrix(params (string key, string[] values)[] map)
    {
        _matrix = new(this, map.ToDictionary());
        return _matrix.Strategy;
    }

    /// <summary>
    /// Set the fail-fast.
    /// See https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#jobsjob_idstrategyfail-fast
    /// </summary>
    /// <param name="failFast"></param>
    /// <returns></returns>
    public Strategy FailFast(bool failFast)
    {
        _failFast = failFast;
        return this;
    }

    /// <summary>
    /// Set the maximum number of parallel jobs to run.
    /// See https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#jobsjob_idstrategymax-parallel 
    /// </summary>
    /// <param name="maxParallel"></param>
    /// <returns></returns>
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