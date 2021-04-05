using System.Text.Json;
using System.Text.Json.Serialization;
using Pulumi.Automation;

// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo

namespace Logicality.Pulumi.Automation.Aws
{
    public sealed class ProviderEndpoints
    {
        [JsonPropertyName("accessanalyzer")]
        public string? Accessanalyzer { get; set; }

        [JsonPropertyName("acm")]
        public string? Acm { get; set; }

        [JsonPropertyName("acmpca")]
        public string? Acmpca { get; set; }

        [JsonPropertyName("amplify")]
        public string? Amplify { get; set; }

        [JsonPropertyName("apigateway")]
        public string? Apigateway { get; set; }

        [JsonPropertyName("applicationautoscaling")]
        public string? Applicationautoscaling { get; set; }

        [JsonPropertyName("applicationinsights")]
        public string? Applicationinsights { get; set; }

        [JsonPropertyName("appmesh")]
        public string? Appmesh { get; set; }

        [JsonPropertyName("appstream")]
        public string? Appstream { get; set; }

        [JsonPropertyName("appsync")]
        public string? Appsync { get; set; }

        [JsonPropertyName("athena")]
        public string? Athena { get; set; }

        [JsonPropertyName("auditmanager")]
        public string? Auditmanager { get; set; }

        [JsonPropertyName("autoscaling")]
        public string? Autoscaling { get; set; }

        [JsonPropertyName("autoscalingplans")]
        public string? Autoscalingplans { get; set; }

        [JsonPropertyName("backup")]
        public string? Backup { get; set; }

        [JsonPropertyName("batch")]
        public string? Batch { get; set; }

        [JsonPropertyName("budgets")]
        public string? Budgets { get; set; }

        [JsonPropertyName("cloud9")]
        public string? Cloud9 { get; set; }

        [JsonPropertyName("cloudformation")]
        public string? Cloudformation { get; set; }

        [JsonPropertyName("cloudfront")]
        public string? Cloudfront { get; set; }

        [JsonPropertyName("cloudhsm")]
        public string? Cloudhsm { get; set; }

        [JsonPropertyName("cloudsearch")]
        public string? Cloudsearch { get; set; }

        [JsonPropertyName("cloudtrail")]
        public string? Cloudtrail { get; set; }

        [JsonPropertyName("cloudwatch")]
        public string? Cloudwatch { get; set; }

        [JsonPropertyName("cloudwatchevents")]
        public string? Cloudwatchevents { get; set; }

        [JsonPropertyName("cloudwatchlogs")]
        public string? Cloudwatchlogs { get; set; }

        [JsonPropertyName("codeartifact")]
        public string? Codeartifact { get; set; }

        [JsonPropertyName("codebuild")]
        public string? Codebuild { get; set; }

        [JsonPropertyName("codecommit")]
        public string? Codecommit { get; set; }

        [JsonPropertyName("codedeploy")]
        public string? Codedeploy { get; set; }

        [JsonPropertyName("codepipeline")]
        public string? Codepipeline { get; set; }

        [JsonPropertyName("codestarconnections")]
        public string? Codestarconnections { get; set; }

        [JsonPropertyName("cognitoidentity")]
        public string? Cognitoidentity { get; set; }

        [JsonPropertyName("cognitoidp")]
        public string? Cognitoidp { get; set; }

        [JsonPropertyName("configservice")]
        public string? Configservice { get; set; }

        [JsonPropertyName("connect")]
        public string? Connect { get; set; }

        [JsonPropertyName("cur")]
        public string? Cur { get; set; }

        [JsonPropertyName("dataexchange")]
        public string? Dataexchange { get; set; }

        [JsonPropertyName("datapipeline")]
        public string? Datapipeline { get; set; }

        [JsonPropertyName("datasync")]
        public string? Datasync { get; set; }

        [JsonPropertyName("dax")]
        public string? Dax { get; set; }

        [JsonPropertyName("devicefarm")]
        public string? Devicefarm { get; set; }

        [JsonPropertyName("directconnect")]
        public string? Directconnect { get; set; }

        [JsonPropertyName("dlm")]
        public string? Dlm { get; set; }

        [JsonPropertyName("dms")]
        public string? Dms { get; set; }

        [JsonPropertyName("docdb")]
        public string? Docdb { get; set; }

        [JsonPropertyName("ds")]
        public string? Ds { get; set; }

        [JsonPropertyName("dynamodb")]
        public string? Dynamodb { get; set; }

        [JsonPropertyName("ec2")]
        public string? Ec2 { get; set; }

        [JsonPropertyName("ecr")]
        public string? Ecr { get; set; }

        [JsonPropertyName("ecrpublic")]
        public string? Ecrpublic { get; set; }

        [JsonPropertyName("ecs")]
        public string? Ecs { get; set; }

        [JsonPropertyName("efs")]
        public string? Efs { get; set; }

        [JsonPropertyName("eks")]
        public string? Eks { get; set; }

        [JsonPropertyName("elasticache")]
        public string? Elasticache { get; set; }

        [JsonPropertyName("elasticbeanstalk")]
        public string? Elasticbeanstalk { get; set; }

        [JsonPropertyName("elastictranscoder")]
        public string? Elastictranscoder { get; set; }

        [JsonPropertyName("elb")]
        public string? Elb { get; set; }

        [JsonPropertyName("emr")]
        public string? Emr { get; set; }

        [JsonPropertyName("emrcontainers")]
        public string? Emrcontainers { get; set; }

        [JsonPropertyName("es")]
        public string? Es { get; set; }

        [JsonPropertyName("firehose")]
        public string? Firehose { get; set; }

        [JsonPropertyName("fms")]
        public string? Fms { get; set; }

        [JsonPropertyName("forecast")]
        public string? Forecast { get; set; }

        [JsonPropertyName("fsx")]
        public string? Fsx { get; set; }

        [JsonPropertyName("gamelift")]
        public string? Gamelift { get; set; }

        [JsonPropertyName("glacier")]
        public string? Glacier { get; set; }

        [JsonPropertyName("globalaccelerator")]
        public string? Globalaccelerator { get; set; }

        [JsonPropertyName("glue")]
        public string? Glue { get; set; }

        [JsonPropertyName("greengrass")]
        public string? Greengrass { get; set; }

        [JsonPropertyName("guardduty")]
        public string? Guardduty { get; set; }

        [JsonPropertyName("iam")]
        public string? Iam { get; set; }

        [JsonPropertyName("identitystore")]
        public string? Identitystore { get; set; }

        [JsonPropertyName("imagebuilder")]
        public string? Imagebuilder { get; set; }

        [JsonPropertyName("inspector")]
        public string? Inspector { get; set; }

        [JsonPropertyName("iot")]
        public string? Iot { get; set; }

        [JsonPropertyName("iotanalytics")]
        public string? Iotanalytics { get; set; }

        [JsonPropertyName("iotevents")]
        public string? Iotevents { get; set; }

        [JsonPropertyName("kafka")]
        public string? Kafka { get; set; }

        [JsonPropertyName("kinesis")]
        public string? Kinesis { get; set; }

        [JsonPropertyName("kinesisanalytics")]
        public string? Kinesisanalytics { get; set; }

        [JsonPropertyName("kinesisanalyticsv2")]
        public string? Kinesisanalyticsv2 { get; set; }

        [JsonPropertyName("kinesisvideo")]
        public string? Kinesisvideo { get; set; }

        [JsonPropertyName("kms")]
        public string? Kms { get; set; }

        [JsonPropertyName("lakeformation")]
        public string? Lakeformation { get; set; }

        [JsonPropertyName("lambda")]
        public string? Lambda { get; set; }

        [JsonPropertyName("lexmodels")]
        public string? Lexmodels { get; set; }

        [JsonPropertyName("licensemanager")]
        public string? Licensemanager { get; set; }

        [JsonPropertyName("lightsail")]
        public string? Lightsail { get; set; }

        [JsonPropertyName("macie")]
        public string? Macie { get; set; }

        [JsonPropertyName("macie2")]
        public string? Macie2 { get; set; }

        [JsonPropertyName("managedblockchain")]
        public string? Managedblockchain { get; set; }

        [JsonPropertyName("marketplacecatalog")]
        public string? Marketplacecatalog { get; set; }

        [JsonPropertyName("mediaconnect")]
        public string? Mediaconnect { get; set; }

        [JsonPropertyName("mediaconvert")]
        public string? Mediaconvert { get; set; }

        [JsonPropertyName("medialive")]
        public string? Medialive { get; set; }

        [JsonPropertyName("mediapackage")]
        public string? Mediapackage { get; set; }

        [JsonPropertyName("mediastore")]
        public string? Mediastore { get; set; }

        [JsonPropertyName("mediastoredata")]
        public string? Mediastoredata { get; set; }

        [JsonPropertyName("mq")]
        public string? Mq { get; set; }

        [JsonPropertyName("mwaa")]
        public string? Mwaa { get; set; }

        [JsonPropertyName("neptune")]
        public string? Neptune { get; set; }

        [JsonPropertyName("networkfirewall")]
        public string? Networkfirewall { get; set; }

        [JsonPropertyName("networkmanager")]
        public string? Networkmanager { get; set; }

        [JsonPropertyName("opsworks")]
        public string? Opsworks { get; set; }

        [JsonPropertyName("organizations")]
        public string? Organizations { get; set; }

        [JsonPropertyName("outposts")]
        public string? Outposts { get; set; }

        [JsonPropertyName("personalize")]
        public string? Personalize { get; set; }

        [JsonPropertyName("pinpoint")]
        public string? Pinpoint { get; set; }

        [JsonPropertyName("pricing")]
        public string? Pricing { get; set; }

        [JsonPropertyName("qldb")]
        public string? Qldb { get; set; }

        [JsonPropertyName("quicksight")]
        public string? Quicksight { get; set; }

        [JsonPropertyName("ram")]
        public string? Ram { get; set; }

        [JsonPropertyName("rds")]
        public string? Rds { get; set; }

        [JsonPropertyName("redshift")]
        public string? Redshift { get; set; }

        [JsonPropertyName("resourcegroups")]
        public string? Resourcegroups { get; set; }

        [JsonPropertyName("resourcegroupstaggingapi")]
        public string? Resourcegroupstaggingapi { get; set; }

        [JsonPropertyName("route53")]
        public string? Route53 { get; set; }

        [JsonPropertyName("route53domains")]
        public string? Route53domains { get; set; }

        [JsonPropertyName("route53resolver")]
        public string? Route53resolver { get; set; }

        [JsonPropertyName("s3")]
        public string? S3 { get; set; }

        [JsonPropertyName("s3control")]
        public string? S3control { get; set; }

        [JsonPropertyName("s3outposts")]
        public string? S3outposts { get; set; }

        [JsonPropertyName("sagemaker")]
        public string? Sagemaker { get; set; }

        [JsonPropertyName("sdb")]
        public string? Sdb { get; set; }

        [JsonPropertyName("secretsmanager")]
        public string? Secretsmanager { get; set; }

        [JsonPropertyName("securityhub")]
        public string? Securityhub { get; set; }

        [JsonPropertyName("serverlessrepo")]
        public string? Serverlessrepo { get; set; }

        [JsonPropertyName("servicecatalog")]
        public string? Servicecatalog { get; set; }

        [JsonPropertyName("servicediscovery")]
        public string? Servicediscovery { get; set; }

        [JsonPropertyName("servicequotas")]
        public string? Servicequotas { get; set; }

        [JsonPropertyName("ses")]
        public string? Ses { get; set; }

        [JsonPropertyName("shield")]
        public string? Shield { get; set; }

        [JsonPropertyName("signer")]
        public string? Signer { get; set; }

        [JsonPropertyName("sns")]
        public string? Sns { get; set; }

        [JsonPropertyName("sqs")]
        public string? Sqs { get; set; }

        [JsonPropertyName("ssm")]
        public string? Ssm { get; set; }

        [JsonPropertyName("ssoadmin")]
        public string? Ssoadmin { get; set; }

        [JsonPropertyName("stepfunctions")]
        public string? Stepfunctions { get; set; }

        [JsonPropertyName("storagegateway")]
        public string? Storagegateway { get; set; }

        [JsonPropertyName("sts")]
        public string? Sts { get; set; }

        [JsonPropertyName("swf")]
        public string? Swf { get; set; }

        [JsonPropertyName("synthetics")]
        public string? Synthetics { get; set; }

        [JsonPropertyName("timestreamwrite")]
        public string? Timestreamwrite { get; set; }

        [JsonPropertyName("transfer")]
        public string? Transfer { get; set; }

        [JsonPropertyName("waf")]
        public string? Waf { get; set; }

        [JsonPropertyName("wafregional")]
        public string? Wafregional { get; set; }

        [JsonPropertyName("wafv2")]
        public string? Wafv2 { get; set; }

        [JsonPropertyName("worklink")]
        public string? Worklink { get; set; }

        [JsonPropertyName("workmail")]
        public string? Workmail { get; set; }

        [JsonPropertyName("workspaces")]
        public string? Workspaces { get; set; }

        [JsonPropertyName("xray")]
        public string? Xray { get; set; }

        public ConfigValue AsConfigValue()
        {
            var options = new JsonSerializerOptions()
            {
                IgnoreNullValues = true
            };
            var json = JsonSerializer.Serialize(this, options);
            return new ConfigValue(json);
        }
    }
}