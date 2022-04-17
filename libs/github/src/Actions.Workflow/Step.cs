using YamlDotNet.Core.Events;
using YamlDotNet.RepresentationModel;

namespace Logicality.GitHub.Actions.Workflow;

public class Step
{
    private readonly string?   _id;
    private          string    _name        = string.Empty;
    private          string    _conditional = string.Empty;
    private          string    _uses        = string.Empty;
    private          StepWith? _with;
    private          string    _run            = string.Empty;
    private          string    _shell          = string.Empty;
    private          int       _timeoutMinutes = 0;
    private          bool      _continueOnError;

    internal Step(string? id, Job job)
    {
        _id = id;
        Job   = job;
    }

    public Job      Job      { get; }
    public Workflow Workflow => Job.Workflow;

    public Step Name(string name)
    {
        _name = name;
        return this;
    }

    public Step If(string conditional)
    {
        _conditional = conditional;
        return this;
    }

    public Step Uses(string uses)
    {
        _uses = uses;
        return this;
    }

    /// <summary>
    /// Excluded if Uses is not specified.
    /// </summary>
    /// <returns></returns>
    public StepWith With()
    {
        _with = new StepWith(this);
        return _with;
    }

    public Step Run(string run)
    {
        _run = run;
        return this;
    }

    public Step Shell(string shell)
    {
        _shell = shell;
        return this;
    }

    public Step TimeoutMinutes(int timeoutMinutes)
    {
        _timeoutMinutes = timeoutMinutes;
        return this;
    }

    public Step ContinueOnError(bool continueOnError)
    {
        _continueOnError = continueOnError;
        return this;
    }

    internal void Build(YamlMappingNode node, SequenceStyle sequenceStyle)
    {
        if (!string.IsNullOrWhiteSpace(_id))
        {
            node.Add("id", _id);
        }

        if (!string.IsNullOrWhiteSpace(_name))
        {
            node.Add("name", _name);
        }

        if (!string.IsNullOrWhiteSpace(_conditional))
        {
            node.Add("if", _conditional);
        }

        if (_timeoutMinutes > 0)
        {
            node.Add("timeout-minutes", _timeoutMinutes.ToString());
        }

        if (_continueOnError)
        {
            node.Add("continue-on-error", _continueOnError.ToString().ToLower());
        }

        if (!string.IsNullOrWhiteSpace(_run))
        {
            node.Add("run", _run);
        }

        if (!string.IsNullOrWhiteSpace(_shell))
        {
            node.Add("shell", _shell);
        }

        if (!string.IsNullOrWhiteSpace(_uses))
        {
            node.Add("uses", _uses);
        }

        _with?.Build(node, sequenceStyle);
    }
}