using System;

namespace Logicality.EventSourcing.Domain.Testing;

public class ScenarioReturnedOtherResult
{
    public Scenario Scenario { get; }

    public object Actual { get; }

    internal ScenarioReturnedOtherResult(Scenario scenario, object actual)
    {
        Scenario = scenario ?? throw new ArgumentNullException(nameof(scenario));
        Actual   = actual;
    }
}