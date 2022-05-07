using System;
using System.Collections.Generic;

namespace Logicality.EventSourcing.Domain.Testing;

public class ScenarioRecordedOtherEvents
{
    public Scenario Scenario { get; }

    public IReadOnlyCollection<RecordedEvent> Actual { get; }
        
    internal ScenarioRecordedOtherEvents(Scenario scenario, IReadOnlyCollection<RecordedEvent> actual)
    {
        Scenario = scenario ?? throw new ArgumentNullException(nameof(scenario));
        Actual   = actual ?? throw new ArgumentNullException(nameof(actual));
    }
}