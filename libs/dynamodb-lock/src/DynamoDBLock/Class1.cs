using Amazon.DynamoDBv2;
using Microsoft.Extensions.Options;

namespace Logicality.DynamoDBLock;

public class DynamoDBLockClient
{
    public DynamoDBLockClient(IAmazonDynamoDB dynamoDBClient, IOptions<DynamoDBLockClientOptions> options)
    {
        
    }
}
