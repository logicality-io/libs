using System.Threading.Tasks;
using Pulumi;

namespace Pulum.Aws.Tests.Stacks
{
    class Program
    {
        static Task<int> Main() => Deployment.RunAsync<MyStack>();
    }
}

