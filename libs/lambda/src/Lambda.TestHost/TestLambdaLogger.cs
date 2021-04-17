using System.Text;
using Amazon.Lambda.Core;

namespace Logicality.AWS.Lambda.TestHost
{
    public class TestLambdaLogger : ILambdaLogger
    {
        private readonly StringBuilder _logs = new StringBuilder();

        public void Log(string message) => _logs.Append(message);

        public void LogLine(string message) => _logs.AppendLine(message);

        public string Logs => _logs.ToString();
    }
}