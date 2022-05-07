using System.Collections.Generic;
using Pulumi.Automation;

namespace Logicality.Pulumi.Automation;

public static class LocalWorkspaceOptionsExtensions
{
    /// <summary>
    /// Configures a <see cref="LocalWorkspaceOptions"/> for use with a local backend. Useful for portable
    /// integration testing.
    /// </summary>
    /// <param name="options"></param>
    public static LocalWorkspaceOptions ConfigureForLocalBackend(this LocalWorkspaceOptions options)
    {
        options.EnvironmentVariables ??= new Dictionary<string, string?>();
        options.EnvironmentVariables.Add(EnvironmentVariableKeys.ConfigPassphrase, "secret");
        options.EnvironmentVariables.Add(EnvironmentVariableKeys.ConfigBackendUrl, "file://~");
        return options;
    }
}