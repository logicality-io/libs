using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Pulumi;
using Pulumi.Automation;

namespace Logicality.Pulumi.Automation
{
    public static class WorkspaceExtensions
    {
        /// <summary>
        /// Installs the correct version of the plug-in related to the TProvider type.
        /// </summary>
        /// <typeparam name="TProvider">The provider type.</typeparam>
        /// <param name="workspace">The workspace to install the plugin to.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public static async Task InstallPluginAsync<TProvider>(this Workspace workspace, CancellationToken cancellationToken = default)
            where TProvider: ProviderResource
        {
            var assembly = typeof(TProvider).Assembly;
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            var pluginName = assembly.GetName().Name!.Replace("Pulumi.", "").ToLower();
            var pluginVersion = $"v{fileVersionInfo.FileMajorPart}.{fileVersionInfo.FileMinorPart}.{fileVersionInfo.FileBuildPart}";

            await workspace.InstallPluginAsync(pluginName, pluginVersion, cancellationToken: cancellationToken);
        }
    }
}