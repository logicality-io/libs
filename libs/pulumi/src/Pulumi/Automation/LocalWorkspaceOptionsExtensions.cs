using System.Collections.Generic;
using Pulumi.Automation;

namespace Logicality.Pulumi.Automation
{
    public static class LocalWorkspaceOptionsExtensions
    {
        /// <summary>
        /// Configures a <see cref="LocalWorkspaceOptions"/> for use with a local backend. Useful for portable
        /// integration testing.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="projectName"></param>
        public static LocalWorkspaceOptions ConfigureForLocalBackend(this LocalWorkspaceOptions options, string projectName)
        {
            options.ProjectSettings = new ProjectSettings(projectName, ProjectRuntimeName.Dotnet);
            options.EnvironmentVariables = new Dictionary<string, string?>
            {
                { EnvironmentVariableKeys.ConfigPassphrase, "secret" },
                { EnvironmentVariableKeys.ConfigBackendUrl, "file://~" }
            };
            return options;
        }
    }
}