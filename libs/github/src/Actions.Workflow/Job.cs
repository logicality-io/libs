using YamlDotNet.Core.Events;
using YamlDotNet.RepresentationModel;

namespace Logicality.GitHub.Actions.Workflow;

/// <summary>
/// Represents a workflow job.
/// </summary>
public class Job
{
    private          string?                        _name;
    private          string[]?                      _needs;
    private          string?                        _runsOn;
    private readonly Dictionary<string, Permission> _permissions                 = new();
    private          PermissionConfig               _permissionConfig            = PermissionConfig.NotSpecified;
    private          string                         _environmentName             = string.Empty;
    private          string                         _environmentUrl              = string.Empty;
    private          string                         _concurrencyGroup            = string.Empty;
    private          bool                           _concurrencyCancelInProgress = false;
    private          JobOutputs?                    _outputs;
    private          JobEnv?                        _env;
    private          JobDefaults?                   _defaults;
    private          string                         _if              = string.Empty;
    private          bool?                          _continueOnError = null;
    private readonly List<Step>                     _steps           = new();
    private          int                            _timeoutMinutes  = 0;
    private          Strategy?                      _strategy        = null;
    private          string                         _uses            = string.Empty;
    private          JobWith?                       _with;
    private          JobSecrets?                    _secrets;
    private          JobContainer?                  _container;
    private          JobServices?                   _services;

    internal Job(string id, Workflow workflow)
    {
        Id       = id;
        Workflow = workflow;
    }

    /// <summary>
    /// The job identifier.
    /// </summary>
    public string   Id       { get; }

    /// <summary>
    /// The parent workflow.
    /// </summary>
    public Workflow Workflow { get; }

    /// <summary>
    /// Set the job name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>The job.</returns>
    public Job Name(string name)
    {
        _name = name;
        return this;
    }

    /// <summary>
    /// Identify any jobs that must complete successfully before this job will run.
    ///
    /// https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#jobsjob_idneeds
    /// </summary>
    /// <param name="needs">The job identifiers.</param>
    /// <returns>The job.</returns>
    public Job Needs(params string[] needs)
    {
        _needs = needs;
        return this;
    }


    /// <summary>
    /// Define the type of machine to run the job on.
    /// https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#jobsjob_idruns-on
    ///  </summary>
    /// <param name="runsOn">Github Hosted runner label. <see cref="GitHubHostedRunners"/>.</param>
    /// <returns>The job.</returns>
    public Job RunsOn(string runsOn)
    {
        _runsOn = runsOn;
        return this;
    }

    /// <summary>
    /// Configure job permissions. See https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#permissions
    /// </summary>
    /// <param name="actions">Actions permission. Default is 'None'.</param>
    /// <param name="checks">Checks permission. Default is 'None'.</param>
    /// <param name="contents">Contents permission. Default is 'None'.</param>
    /// <param name="deployments">Deployments permission. Default is 'None'.</param>
    /// <param name="discussions">Discussion permission. Default is 'None'.</param>
    /// <param name="idToken">Id Token permission. Default is 'None'.</param>
    /// <param name="issues">Issues permission. Defualt is 'None'.</param>
    /// <param name="packages">Packages permission. Default is 'None'.</param>
    /// <param name="pages">Pages permission. Default is 'None'.</param>
    /// <param name="pullRequests">Pull requests permission. Default is 'None'.</param>
    /// <param name="repositoryProjects">Repository projects permission. Default is 'None'.</param>
    /// <param name="securityEvents">Security events permission. Default is 'None'.</param>
    /// <param name="statuses">Status permission. Default is 'None'.</param>
    /// <returns>The job.</returns>
    public Job Permissions(
        Permission actions            = Permission.None,
        Permission checks             = Permission.None,
        Permission contents           = Permission.None,
        Permission deployments        = Permission.None,
        Permission discussions        = Permission.None,
        Permission idToken            = Permission.None,
        Permission issues             = Permission.None,
        Permission packages           = Permission.None,
        Permission pages              = Permission.None,
        Permission pullRequests       = Permission.None,
        Permission repositoryProjects = Permission.None,
        Permission securityEvents     = Permission.None,
        Permission statuses           = Permission.None)
    {
        _permissions[PermissionKeys.Actions]            = actions;
        _permissions[PermissionKeys.Checks]             = checks;
        _permissions[PermissionKeys.Contents]           = contents;
        _permissions[PermissionKeys.Deployments]        = deployments;
        _permissions[PermissionKeys.Discussions]        = discussions;
        _permissions[PermissionKeys.IdToken]            = idToken;
        _permissions[PermissionKeys.Issues]             = issues;
        _permissions[PermissionKeys.Packages]           = packages;
        _permissions[PermissionKeys.Pages]              = pages;
        _permissions[PermissionKeys.PullRequests]       = pullRequests;
        _permissions[PermissionKeys.RepositoryProjects] = repositoryProjects;
        _permissions[PermissionKeys.SecurityEvents]     = securityEvents;
        _permissions[PermissionKeys.Statuses]           = statuses;

        _permissionConfig = PermissionConfig.Custom;

        return this;
    }

    /// <summary>
    /// Set job permissions to `read-all`. See https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#permissions
    /// </summary>
    /// <returns>The workflow.</returns>
    public Job PermissionsReadAll()
    {
        _permissionConfig = PermissionConfig.ReadAll;
        return this;
    }

    /// <summary>
    /// Set job permissions to `write-all`.
    /// See https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#permissions
    /// </summary>
    /// <returns>The job..</returns>
    public Job PermissionsWriteAll()
    {
        _permissionConfig = PermissionConfig.WriteAll;
        return this;
    }

    /// <summary>
    /// Define the environment that the job references. 
    /// See https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#jobsjob_idenvironment
    /// </summary>
    /// <param name="name">The environment name.</param>
    /// <param name="url">The environment url.</param>
    /// <returns>The job</returns>
    public Job Environment(string name, string url)
    {
        _environmentName = name;
        _environmentUrl  = url;
        return this;
    }

    /// <summary>
    /// Set workflow concurrency. See https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#concurrency
    /// </summary>
    /// <param name="group"></param>
    /// <param name="cancelInProgress"></param>
    /// <returns>The workflow.</returns>
    public Job Concurrency(string @group, bool cancelInProgress = false)
    {
        _concurrencyGroup            = @group;
        _concurrencyCancelInProgress = cancelInProgress;
        return this;
    }

    /// <summary>
    /// Create a map of outputs for a jobs.
    /// </summary>
    /// <param name="map">The output map.</param>
    /// <returns>The Job.</returns>
    public Job Outputs(params (string key, string value)[] map)
    {
        _outputs = new(this, map.ToDictionary());
        return _outputs.Job;
    }

    /// <summary>
    /// A map of environment variables that are available to the job.
    /// See https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#env
    /// </summary>
    /// <param name="map"></param>
    /// <returns>The job.</returns>
    public Job Env(params (string key, string value)[] map)
    {
        _env = new(this, map.ToDictionary());
        return _env.Job;
    }

    /// <summary>
    /// Create a map of default settings that will apply to all steps in the job.
    /// See https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#jobsjob_iddefaults
    /// </summary>
    /// <returns>A JobDefaults object.</returns>
    public JobDefaults Defaults()
    {
        _defaults = new(this);
        return _defaults;
    }

    /// <summary>
    /// Create a map of default settings that will apply to all steps in the job.
    /// See https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#jobsjob_iddefaults
    /// </summary>
    /// <param name="map"></param>
    /// <returns>A JobDefaults object.</returns>
    public JobDefaults Defaults(params (string key, string value)[] map)
    {
        _defaults = new(this, map.ToDictionary());
        return _defaults;
    }

    /// <summary>
    /// Set conditional to prevent a job from running unless a condition is met.
    /// See https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#jobsjob_idif
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    public Job If(string condition)
    {
        _if = condition;
        return this;
    }

    /// <summary>
    /// The maximum number of minutes to run the job.
    /// See https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#jobsjob_idtimeout-minutes
    /// </summary>
    /// <param name="minutes"></param>
    /// <returns></returns>
    public Job TimeoutMinutes(int minutes)
    {
        _timeoutMinutes = minutes;
        return this;
    }

    /// <summary>
    /// Create a build matrix for the job.
    /// See https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#jobsjob_idstrategy
    /// </summary>
    /// <returns></returns>
    public Strategy Strategy()
    {
        _strategy = new Strategy(this);
        return _strategy;
    }

    /// <summary>
    /// Prevents a workflow run from failing when a job fails.
    /// See https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#jobsjob_idcontinue-on-error
    /// </summary>
    /// <param name="continueOnError"></param>
    /// <returns></returns>
    public Job ContinueOnError(bool continueOnError)
    {
        _continueOnError = continueOnError;
        return this;
    }

    /// <summary>
    /// The location and version of a reusable workflow file to run as a job.
    /// See https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#jobsjob_iduses
    /// </summary>
    /// <param name="uses"></param>
    /// <returns></returns>
    public Job Uses(string uses)
    {
        _uses = uses;
        return this;
    }

    /// <summary>
    /// Provide a map of inputs that are passed to the called workflow. Ignored if 'Uses' is not specified.
    /// https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#jobsjob_idwith
    /// </summary>
    /// <returns>The Job</returns>
    public Job With(params (string key, string value)[] map)
    {
        _with = new(this, map.ToDictionary());
        return _with.Job;
    }

    /// <summary>
    /// Create a container to run any steps in a job that don't already specify a container.
    /// See https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#jobsjob_idcontainer
    /// </summary>
    /// <param name="image"></param>
    /// <returns>A JobContainer to configure the container.</returns>
    public JobContainer Container(string image)
    {
        _container = new(this, image);
        return _container;
    }

    /// <summary>
    /// Define service containers for a job in a workflow
    /// See https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#jobsjob_idservices
    /// </summary>
    /// <returns>A JobServices object to configure the services.</returns>
    public JobServices Services()
    {
        _services = new(this);
        return _services;
    }

    /// <summary>
    /// Specify a map of secrets that are passed to the called workflow.
    /// See https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#jobsjob_idsecrets
    /// </summary>
    /// <param name="map"></param>
    /// <returns></returns>
    public JobSecrets Secrets(params (string key, string value)[] map)
    {
        _secrets = new(this, map.ToDictionary());
        return _secrets;
    }

    /// <summary>
    /// Add a Step to the Job.
    /// </summary>
    /// <param name="id">The step identifier.</param>
    /// <returns>Return a Step object to configure a step.</returns>
    public Step Step(string? id = null)
    {
        var step = new Step(id, this);
        _steps.Add(step);
        return step;
    }

    internal void Build(YamlMappingNode node, SequenceStyle sequenceStyle)
    {
        var jobNode = new YamlMappingNode();
        if (!string.IsNullOrWhiteSpace(_name))
        {
            jobNode.Add("name", _name);
        }

        // If
        if (!string.IsNullOrWhiteSpace(_if))
        {
            jobNode.Add("if", _if);
        }

        // Needs
        if (_needs != null && _needs.Any())
        {
            var needsSequenceNode = new YamlSequenceNode(_needs.Select(s => new YamlScalarNode(s)))
            {
                Style = sequenceStyle
            };
            jobNode.Add("needs", needsSequenceNode);
        }

        // RunsOn
        if (!string.IsNullOrWhiteSpace(_runsOn))
        {
            jobNode.Add("runs-on", _runsOn);
        }

        // Concurrency
        if (!string.IsNullOrWhiteSpace(_concurrencyGroup))
        {
            var concurrencyMappingNode = new YamlMappingNode
            {
                { "group", _concurrencyGroup },
                { "cancel-in-progress", _concurrencyCancelInProgress.ToString().ToLower() }
            };
            jobNode.Add("concurrency", concurrencyMappingNode);
        }

        // Continue On Error
        if (_continueOnError.HasValue)
        {
            jobNode.Add("continue-on-error", _continueOnError.Value.ToString().ToLower());
        }

        // Permission
        if (_permissionConfig != PermissionConfig.NotSpecified)
        {
            PermissionHelper.BuildPermissionsNode(jobNode, _permissionConfig, _permissions);
        }
        
        // Defaults
        _defaults?.Build(jobNode);
        
        // Env
        _env?.Build(jobNode);
       
        // Environment
        if (!string.IsNullOrWhiteSpace(_environmentName))
        {
            var environmentMappingNode = new YamlMappingNode
            {
                { "name", _environmentName }
            };
            if (!string.IsNullOrWhiteSpace(_environmentUrl))
            {
                environmentMappingNode.Add("url", _environmentUrl);
            }
            jobNode.Add("environment", environmentMappingNode);
        }
        
        // Uses
        if (!string.IsNullOrWhiteSpace(_uses))
        {
            jobNode.Add("uses", _uses);

            if (_with != null)
            {
                _with.Build(jobNode);
            }
        }

        // Strategy
        _strategy?.Build(jobNode, sequenceStyle);

        if (_timeoutMinutes > 0)
        {
            jobNode.Add("timeout-minutes", _timeoutMinutes.ToString());
        }

        // Container
        _container?.Build(jobNode, sequenceStyle);

        // Services
        _services?.Build(jobNode, sequenceStyle);

        // Outputs
        _outputs?.Build(jobNode);

        //Secrets
        _secrets?.Build(jobNode);

        // Steps
        if (_steps.Any())
        {
            var sequenceNode = new YamlSequenceNode();
            foreach (var step in _steps)
            {
                var stepMappingNode = new YamlMappingNode();
                step.Build(stepMappingNode, sequenceStyle);
                sequenceNode.Add(stepMappingNode);
            }
            jobNode.Add("steps", sequenceNode);
        }

        node.Add(Id, jobNode);
    }
}