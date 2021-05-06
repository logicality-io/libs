using System.Threading.Tasks;
using Pulumi;
using Pulumi.Automation;

namespace Logicality.Pulumi.Automation
{
    public static class LocalWorkspaceExtensions
    {
        /// <summary>
        /// Installs the plug-in related to the TProvider type.
        /// </summary>
        /// <typeparam name="TProvider"></typeparam>
        /// <param name="localWorkspace"></param>
        /// <returns></returns>
        public static async Task<LocalWorkspace> InstallPluginAsync<TProvider>(this LocalWorkspace localWorkspace)
            where TProvider: ProviderResource
        {
            var assemblyName = typeof(TProvider).Assembly.GetName()!;
            var assemblyVersion = assemblyName.Version!;
            var pluginName = assemblyName.Name!.Replace("Pulumi.", "").ToLower();
            var pluginVersion = $"v{assemblyVersion.Major}.{assemblyVersion.Minor}.{assemblyVersion.Revision}";

            await localWorkspace.InstallPluginAsync(pluginName, pluginVersion);

            return localWorkspace;
        }
    }
}