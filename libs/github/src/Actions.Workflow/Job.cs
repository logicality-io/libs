using System.Net.NetworkInformation;
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
    private          IDictionary<string, string>    _outputs                     = new Dictionary<string, string>();
    private          IDictionary<string, string>    _env                         = new Dictionary<string, string>();
    private          IDictionary<string, string>    _defaults                    = new Dictionary<string, string>();
    private          string                         _defaultsRunShell            = string.Empty;
    private          string                         _defaultsRunWorkingDir       = string.Empty;
    private          string                         _if                          = string.Empty;
    private          bool?                          _continueOnError             = null;
    private readonly List<Step>                     _steps                       = new();
    private          int                            _timeoutMinutes              = 0;
    private          Strategy?                      _strategy                    = null;
    private          string                         _uses                        = string.Empty;
    private readonly List<(string, string)>         _with                        = new();

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

    public Job Outputs(IDictionary<string, string> outputs)
    {
        _outputs = outputs;
        return this;
    }

    public Job Env(IDictionary<string, string> environment)
    {
        _env = environment;
        return this;
    }

    public Job Defaults(IDictionary<string, string> defaults)
    {
        _defaults = defaults;
        return this;
    }

    public Job DefaultsRun(string shell, string workingDirectory)
    {
        _defaultsRunShell      = shell;
        _defaultsRunWorkingDir = workingDirectory;
        return this;
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
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public Job With(string name, string value)
    {
        _with.Add((name, value));
        return this;
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
        if (_defaults.Any() || !string.IsNullOrWhiteSpace(_defaultsRunShell))
        {
            var defaultsMappingNode = new YamlMappingNode();
            foreach (var @default in _defaults)
            {
                defaultsMappingNode.Add(@default.Key, new YamlScalarNode(@default.Value));
            }
            // Defauls Run
            if (!string.IsNullOrWhiteSpace(_defaultsRunShell))
            {
                var defaultsRunMappingNode = new YamlMappingNode()
                {
                    { "shell", _defaultsRunShell },
                    { "working-directory", _defaultsRunWorkingDir }
                };
                defaultsMappingNode.Add("run", defaultsRunMappingNode);
            }
            jobNode.Add("defaults", defaultsMappingNode);
        }

        // Env
        if (_env.Any())
        {
            var envMappingNode = new YamlMappingNode();
            foreach (var env in _env)
            {
                envMappingNode.Add(env.Key, new YamlScalarNode(env.Value));
            }
            jobNode.Add("env", envMappingNode);
        }
       
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

            if (_with.Any())
            {
                var withNode = new YamlMappingNode();
                foreach (var with in _with)
                {
                    withNode.Add(with.Item1, with.Item2);
                }
                jobNode.Add("with", withNode);
            }
        }

        // Strategy
        _strategy?.Build(jobNode, sequenceStyle);

        if (_timeoutMinutes > 0)
        {
            jobNode.Add("timeout-minutes", _timeoutMinutes.ToString());
        }

        // Outputs
        if (_outputs.Any())
        {
            var outputsMappingNode = new YamlMappingNode();
            foreach (var output in _outputs)
            {
                outputsMappingNode.Add(output.Key, output.Value);
            }
            jobNode.Add("outputs", outputsMappingNode);
        }

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