namespace Logicality.Domain.FooEntity;

public static class Commands
{
    public record NewFoo(CommandId CommandId, string Name) : DomainCommand(CommandId);

    public record DoAThing(CommandId CommandId, string Name) : DomainCommand(CommandId);
}