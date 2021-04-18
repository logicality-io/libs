using Amazon.Lambda.Core;
using Xunit.Abstractions;

namespace Logicality.AWS.Lambda.TestHost
{
    internal class XunitLambdaLogger : ILambdaLogger
    {
        private readonly ITestOutputHelper _outputHelper;

        public XunitLambdaLogger(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        public void Log(string message) => _outputHelper.WriteLine(message);

        public void LogLine(string message) => _outputHelper.WriteLine(message);
    }
}