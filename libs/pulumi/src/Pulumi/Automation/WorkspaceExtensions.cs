using System.Threading.Tasks;
using Pulumi;
using Pulumi.Automation;

namespace Logicality.Pulumi.Automation
{
    public static class WorkspaceExtensions
    {
        /// <summary>
        /// Installs the plug-in related to the TProvider type.
        /// </summary>
        /// <typeparam name="TProvider"></typeparam>
        /// <param name="workspace"></param>
        /// <returns></returns>
        public static async Task<Workspace> InstallPluginAsync<TProvider>(this Workspace workspace)
            where TProvider: ProviderResource
        {
            var assemblyName = typeof(TProvider).Assembly.GetName()!;
            var assemblyVersion = assemblyName.Version!;
            var pluginName = assemblyName.Name!.Replace("Pulumi.", "").ToLower();
            var pluginVersion = $"v{assemblyVersion.Major}.{assemblyVersion.Minor}.{assemblyVersion.Revision}";

            await workspace.InstallPluginAsync(pluginName, pluginVersion);

            return workspace;
        }
    }
}