namespace Logicality.EventSourcing.Domain.Testing;

public class ScenarioThrewException(Scenario scenario, Exception exception)
{
    public Scenario Scenario { get; } = scenario;

    public Exception Exception { get; } = exception;
}