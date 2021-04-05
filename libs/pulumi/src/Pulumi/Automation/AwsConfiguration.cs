using System.Collections.Generic;
using Pulumi.Automation;

namespace Logicality.Pulumi.Automation
{
    public static class AwsConfiguration
    {
        /// <summary>
        /// Configuration the AWS provider to be usable for local deployment / development (i.e.
        /// dynamodb-local, localstack etc)
        /// </summary>
        /// <param name="configMap"></param>
        public static void ConfigureForLocal(IDictionary<string, ConfigValue> configMap)
        {
            configMap.Add(AwsConfigurationKeys.Region, new ConfigValue("eu-west-1"));
            configMap.Add(AwsConfigurationKeys.AccessKey, new ConfigValue("ignored"));
            configMap.Add(AwsConfigurationKeys.SecretKey, new ConfigValue("ignored"));
            configMap.Add(AwsConfigurationKeys.SkipCredentialsValidation, new ConfigValue("true"));
            configMap.Add(AwsConfigurationKeys.SkipRequestingAccountId, new ConfigValue("true"));
        }
    }
}