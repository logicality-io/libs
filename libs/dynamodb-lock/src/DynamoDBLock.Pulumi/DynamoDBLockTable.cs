using Pulumi;
using Pulumi.Aws.DynamoDB;
using Pulumi.Aws.DynamoDB.Inputs;

namespace Logicality.DynamoDBLock.Pulumi
{
    public class DynamoDBLockTable : ComponentResource
    {
        public DynamoDBLockTable(string name, TableArgs tableArgs, ComponentResourceOptions? options = null)
            : base("logicality:aws:dynamodb:locktable", name, options)
        {
            Table = new Table("lockTable", tableArgs, new CustomResourceOptions
            {
                Parent = this
            });
        }

        public Table Table { get; }

        public static TableArgs DefaultTableArgs => new TableArgs
        {
            Attributes =
            {
                new TableAttributeArgs
                {
                    Name = "PK",
                    Type = "S",
                },
                new TableAttributeArgs
                {
                    Name = "SK",
                    Type = "S",
                }
            },
            HashKey = "PK",
            RangeKey = "SK",
        };
    }
}
