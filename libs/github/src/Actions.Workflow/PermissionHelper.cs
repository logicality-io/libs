using YamlDotNet.RepresentationModel;

namespace Logicality.GitHub.Actions.Workflow;

internal static class PermissionHelper
{
    internal static void BuildPermissionsNode(
        YamlMappingNode                node,
        PermissionConfig               permissionConfig,
        Dictionary<string, Permission> permissions)
    {
        if (permissionConfig == PermissionConfig.NotSpecified)
        {
            return;
        }

        switch (permissionConfig)
        {
            case PermissionConfig.ReadAll:
                node.Add("permissions", "read-all");
                break;
            case PermissionConfig.WriteAll:
                node.Add("permissions", "write-all");
                break;
            case PermissionConfig.Custom:
            {
                var mappingNode = new YamlMappingNode();
                foreach (var permission in permissions)
                {
                    if (permission.Value != Permission.None)
                    {
                        mappingNode.Add(permission.Key, permission.Value.ToString().ToLower());
                    }
                }
                node.Add("permissions", mappingNode);
                break;
            }
        }
    }
}