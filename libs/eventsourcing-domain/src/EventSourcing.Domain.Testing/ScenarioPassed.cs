namespace Logicality.EventSourcing.Domain.Testing;

public class ScenarioPassed
{
    public Scenario Scenario { get; }

    internal ScenarioPassed(Scenario scenario)
    {
        Scenario = scenario ?? throw new ArgumentNullException(nameof(scenario));
    }
}