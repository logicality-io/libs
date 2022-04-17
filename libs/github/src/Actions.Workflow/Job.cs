using YamlDotNet.Core.Events;
using YamlDotNet.RepresentationModel;

namespace Logicality.GitHub.Actions.Workflow;

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

    public string   Id       { get; }
    public Workflow Workflow { get; }

    public Job Name(string name)
    {
        _name = name;
        return this;
    }

    public Job Needs(params string[] needs)
    {
        _needs = needs;
        return this;
    }

    public Job RunsOn(string runsOn)
    {
        _runsOn = runsOn;
        return this;
    }

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

    public Job PermissionsReadAll()
    {
        _permissionConfig = PermissionConfig.ReadAll;
        return this;
    }

    public Job PermissionsWriteAll()
    {
        _permissionConfig = PermissionConfig.WriteAll;
        return this;
    }

    public Job Environment(string name, string url)
    {
        _environmentName = name;
        _environmentUrl  = url;
        return this;
    }

    public Job Concurrency(string @group, bool cancelInProgress = false)
    {
        _concurrencyGroup            = @group;
        _concurrencyCancelInProgress = cancelInProgress;
        return this;
    }

    public JobOutputs Outputs()
    {
        _outputs = new JobOutputs(this);
        return _outputs;
    }

    public JobOutputs Outputs(IDictionary<string, string> properties)
    {
        _outputs = new JobOutputs(this, properties);
        return _outputs;
    }

    public JobEnv Env()
    {
        _env = new JobEnv(this);
        return _env;
    }

    public JobEnv Env(IDictionary<string, string> properties)
    {
        _env = new JobEnv(this, properties);
        return _env;
    }

    public JobDefaults Defaults()
    {
        _defaults = new JobDefaults(this);
        return _defaults;
    }

    public JobDefaults Defaults(IDictionary<string, string> defaults)
    {
        _defaults = new JobDefaults(this, defaults);
        return _defaults;
    }

    public Job If(string @if)
    {
        _if = @if;
        return this;
    }

    public Job TimeoutMinutes(int minutes)
    {
        _timeoutMinutes = minutes;
        return this;
    }

    public Strategy Strategy()
    {
        _strategy = new Strategy(this);
        return _strategy;
    }

    public Job ContinueOnError(bool continueOnError)
    {
        _continueOnError = continueOnError;
        return this;
    }

    public Job Uses(string uses)
    {
        _uses = uses;
        return this;
    }

    /// <summary>
    /// Excluded if Uses is not specified.
    /// </summary>
    /// <returns></returns>
    public JobWith With()
    {
        _with = new JobWith(this);
        return _with;
    }

    /// <summary>
    /// Excluded if Uses is not specified.
    /// </summary>
    /// <returns></returns>
    public JobWith With(IDictionary<string, string> properties)
    {
        _with = new JobWith(this, properties);
        return _with;
    }

    public JobContainer Container(string image)
    {
        _container = new JobContainer(this, image);
        return _container;
    }

    public JobServices Services()
    {
        _services = new JobServices(this);
        return _services;
    }

    public JobSecrets Secrets()
    {
        _secrets = new JobSecrets(this);
        return _secrets;
    }

    public JobSecrets Secrets(IDictionary<string, string> properties)
    {
        _secrets = new JobSecrets(this, properties);
        return _secrets;
    }

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