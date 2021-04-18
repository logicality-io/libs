using System;
using System.Collections.Generic;
using System.Threading;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Logging;

namespace Logicality.AWS.Lambda.TestHost
{
    public class LambdaTestHostSettings
    {
        private readonly Dictionary<string, LambdaFunctionInfo> _functions = new Dictionary<string, LambdaFunctionInfo>();

        /// <summary>
        /// The URL the lambda test host will listen on. Default value is http://127.0.0.1:0
        /// which will listen on a random free port. To get the URL to invoke lambdas, use
        /// LambdaTestHost.ServiceUrl.
        /// </summary>
        public string WebHostUrl { get; set; } = "http://*:0";

        public LambdaTestHostSettings(Func<ILambdaContext> createContext)
        {
            CreateContext = createContext;
        }

        /// <summary>
        /// Gets or sets the maximum concurrency limit for all hosted lambdas.
        /// </summary>
        public uint AccountConcurrencyLimit { get; set; } = 1000;

        internal Func<ILambdaContext> CreateContext { get; }

        public Action<ILoggingBuilder> ConfigureLogging { get; set; } = _ => { };

        public IReadOnlyDictionary<string, LambdaFunctionInfo> Functions => _functions;

            //Used in tests to signal the start of an invocation.
        internal AutoResetEvent InvocationOnStart => new AutoResetEvent(false);

        public void AddFunction(LambdaFunctionInfo lambdaFunctionInfo)
        {
            _functions.Add(lambdaFunctionInfo.Name, lambdaFunctionInfo);
        }
    }
}