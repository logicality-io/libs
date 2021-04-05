namespace Logicality.Pulumi.Automation
{
    public static class EnvironmentVariableKeys 
    {
        public const string ConfigPassphrase = "PULUMI_CONFIG_PASSPHRASE";
        public const string ConfigBackendUrl = "PULUMI_BACKEND_URL";
    }

    public static class AwsConfigurationKeys
    {
        public const string Region = "aws:region";
        public const string AccessKey = "aws:accessKey";
        public const string SecretKey = "aws:secretKey";
        public const string SkipCredentialsValidation = "aws:skipCredentialsValidation";
        public const string SkipRequestingAccountId = "aws:skipRequestingAccountId";
    }
}
