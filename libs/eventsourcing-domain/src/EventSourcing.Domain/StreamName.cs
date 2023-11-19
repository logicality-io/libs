namespace Logicality.EventSourcing.Domain;

public record StreamName(string Value)
{
    public override string ToString() => Value;
}