using Amazon.Lambda.Core;
using Xunit.Abstractions;

namespace Logicality.Lambda.TestUtilities;

public class XunitLambdaLogger(ITestOutputHelper outputHelper) : ILambdaLogger
{
    public void Log(string message) => outputHelper.WriteLine(message);

    public void LogLine(string message) => outputHelper.WriteLine(message);
}