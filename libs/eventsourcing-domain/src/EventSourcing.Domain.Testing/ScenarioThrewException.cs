using System;

namespace Logicality.EventSourcing.Domain.Testing
{
    public class ScenarioThrewException
    {
        public ScenarioThrewException(Scenario scenario, Exception exception)
        {
            Scenario = scenario;
            Exception = exception;
        }

        public Scenario Scenario { get; }

        public Exception Exception { get; }
    }
}
