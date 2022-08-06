using YamlDotNet.Core.Events;
using YamlDotNet.RepresentationModel;

namespace Logicality.GitHub.Actions.Workflow;

public class Step
{
    private readonly string?   _id;
    private          string    _name        = string.Empty;
    private          string    _conditional = string.Empty;
    private          StepEnv?  _env;
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

    /// <summary>
    /// A name for your step to display on GitHub.
    /// See https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#jobsjob_idstepsname
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Step Name(string name)
    {
        _name = name;
        return this;
    }
    /// <summary>
    /// Specify a conditional to prevent a step from running unless the condition is met.
    /// See https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#jobsjob_idstepsif
    /// </summary>
    /// <param name="conditional"></param>
    /// <returns></returns>
    public Step If(string conditional)
    {
        _conditional = conditional;
        return this;
    }

    /// <summary>
    /// Select an action to run as part of a step in your job
    /// See https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#jobsjob_idstepsuses
    /// </summary>
    /// <param name="uses"></param>
    /// <returns></returns>
    public Step Uses(string uses)
    {
        _uses = uses;
        return this;
    }

    /// <summary>
    /// A map of environment variables that are available to the step.
    /// See https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#env
    /// </summary>
    /// <param name="map"></param>
    /// <returns>The step.</returns>
    public Step Env(params (string Key, string Value)[] map)
    {
        _env = new(this, map.ToDictionary());
        return _env.Step;
    }

    /// <summary>
    /// A map of the input parameters defined by the action. Ignored if Uses is not specified.
    /// See https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#jobsjob_idstepswith
    /// </summary>
    /// <returns></returns>
    public Step With(params (string Key, string Value)[] map)
    {
        var dict = map.ToDictionary();
        _with = new(this, dict);
        return _with.Step;
    }

    /// <summary>
    /// Runs command-line programs using the operating system's shell.
    /// See https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#jobsjob_idstepsrun
    /// </summary>
    /// <param name="run"></param>
    /// <returns></returns>
    public Step Run(string run)
    {
        _run = run;
        return this;
    }

    /// <summary>
    /// Iverride the default shell settings.
    /// See https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#jobsjob_idstepsshell
    /// </summary>
    /// <param name="shell"></param>
    /// <returns></returns>
    public Step Shell(string shell)
    {
        _shell = shell;
        return this;
    }

    /// <summary>
    /// The maximum number of minutes to run the step.
    /// See https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#jobsjob_idstepstimeout-minutes
    /// </summary>
    /// <param name="minutes"></param>
    /// <returns></returns>
    public Step TimeoutMinutes(int minutes)
    {
        _timeoutMinutes = minutes;
        return this;
    }
    /// <summary>
    /// Prevents a job from failing when a step fails.
    /// See https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#jobsjob_idstepscontinue-on-error
    /// </summary>
    /// <param name="continueOnError"></param>
    /// <returns></returns>
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
        
        // Env
        _env?.Build(node);

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