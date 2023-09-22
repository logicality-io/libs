using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Logicality.GitHub.Actions.Workflow;

public static class Contexts
{
    /// <summary>
    /// Information about the workflow run. ${{ github.... }}
    /// </summary>
    public static class GitHub
    {
        /// <summary>
        /// The name of the action currently running, or the id of a step.
        /// </summary>
        public const string Action = "${{ github.action }}";

        /// <summary>
        /// The path where an action is located. This property is only supported in composite actions.
        /// </summary>
        public const string ActionPath = "${{ github.action_path }}";

        /// <summary>
        /// For a step executing an action, this is the ref of the action being executed.
        /// </summary>
        public const string ActionRef = "${{ github.action_ref }}";

        /// <summary>
        /// For a step executing an action, this is the owner and repository name of the action.
        /// </summary>
        public const string ActionRepository = "${{ github.action_repository }}";

        /// <summary>
        /// For a composite action, the current result of the composite action.
        /// </summary>
        public const string ActionStatus = "${{ github.action_status }}";

        /// <summary>
        /// The username of the user that triggered the initial workflow run.
        /// </summary>
        public const string Actor = "${{ github.actor }}";

        /// <summary>
        /// The URL of the GitHub REST API.
        /// </summary>
        public const string ApiUrl = "${{ github.api_url }}";

        /// <summary>
        /// The base_ref or target branch of the pull request in a workflow run.
        /// </summary>
        public const string BaseRef = "${{ github.base_ref }}";

        /// <summary>
        /// Path on the runner to the file that sets environment variables from workflow commands.
        /// </summary>
        public const string Env = "${{ github.env }}";

        /// <summary>
        /// The full event webhook payload.
        /// </summary>
        public const string Event = "${{ github.event }}";

        /// <summary>
        /// The name of the event that triggered the workflow run.
        /// </summary>
        public const string EventName = "${{ github.event_name }}";

        /// <summary>
        /// The path to the file on the runner that contains the full event webhook payload.
        /// </summary>
        public const string EventPath = "${{ github.event_path }}";

        /// <summary>
        /// The URL of the GitHub GraphQL API.
        /// </summary>
        public const string GraphqlUrl = "${{ github.graphql_url }}";

        /// <summary>
        /// The head_ref or source branch of the pull request in a workflow run.
        /// </summary>
        public const string HeadRef = "${{ github.head_ref }}";

        /// <summary>
        /// The job_id of the current job.
        /// </summary>
        public const string Job = "${{ github.job }}";

        /// <summary>
        /// Path on the runner to the file that sets system PATH variables from workflow commands.
        /// </summary>
        public const string Path = "${{ github.path }}";

        /// <summary>
        /// The ref of the branch or tag that triggered the workflow.
        /// </summary>
        public const string Ref = "${{ github.ref }}";

        /// <summary>
        /// The owner and repository name.
        /// </summary>
        public const string Repository = "${{ github.repository }}";

        /// <summary>
        /// The Git URL to the repository.
        /// </summary>
        public const string RepositoryUrl = "${{ github.repositoryUrl }}";

        /// <summary>
        /// The number of days that workflow run logs and artifacts are kept.
        /// </summary>
        public const string RetentionDays = "${{ github.retention_days }}";

        /// <summary>
        /// The unique identifier (id) of the current workflow run.
        /// </summary>
        public const string RunId = "${{ github.run_id }}";

        /// <summary>
        /// A unique number for each attempt of a particular workflow run in a repository.
        /// </summary>
        public const string RunNumber = "${{ github.run_number }}";

        /// <summary>
        /// The source of a secret used in a workflow.
        /// </summary>
        public const string SecretSource = "${{ github.secret_source }}";

        /// <summary>
        /// The URL of the GitHub server.
        /// </summary>
        public const string ServerUrl = "${{ github.server_url }}";

        /// <summary>
        /// The commit SHA that triggered the workflow.
        /// </summary>
        public const string Sha = "${{ github.sha }}";

        /// <summary>
        /// A token to authenticate on behalf of the GitHub App installed on your repository.
        /// </summary>
        public const string Token = "${{ github.token }}";

        /// <summary>
        /// The name of the workflow.
        /// </summary>
        public const string Workflow = "${{ github.workflow }}";

        /// <summary>
        /// The default working directory on the runner for steps, and the default location of your repository when using the checkout action.
        /// </summary>
        public const string Workspace = "${{ github.workspace }}";
    }

    public static string Env(string environmentVarialeName) => $"${{ env.{environmentVarialeName} }}";
    
    public static string Vars(string variableName) => $"${{ vars.{variableName} }}";

    public static class Job
    {
        /// <summary>
        /// Information about the job's container.
        /// </summary>
        public const string Container = "${{ job.container }}";

        /// <summary>
        /// The ID of the container.
        /// </summary>
        public const string ContainerId = "${{ job.container.id }}";

        /// <summary>
        /// The ID of the container network. The runner creates the network used by all containers in a job.
        /// </summary>
        public const string ContainerNetwork = "${{ job.container.newtork }}";

        /// <summary>
        /// The service containers created for a job.
        /// </summary>
        public const string Services = "${{ job.services }}";

        /// <summary>
        /// The ID of the service container.
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        public static JobServices Services(string serviceId)=> new(serviceId);
        
        public class JobServices(string serviceId)
        {
            /// <summary>
            /// The ID of the service container.
            /// </summary>
            public string Id => $"${{ job.services.{serviceId}.id }}";
            /// <summary>
            /// The ID of the service container network. The runner creates the network used by all containers in a job.
            /// </summary>
            public string Network => $"${{ job.services.{serviceId}.network}}";
            /// <summary>
            /// The ID of the service container network. The runner creates the network used by all containers in a job.
            /// </summary>
            public string Port => $"${{ job.services.{serviceId}.port}}";
        }
    }

    public static class Jobs{}

    public static class Steps
    {
    }

    public static class Runner
    {

    }

    public static class Secrets
    {

    }
    public static class Strategy{}

    public static class Matrix{}

    public static class Needs{}

    public static class Inputs{}
}