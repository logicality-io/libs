namespace Logicality.Domain.FooEntity;

public static class Events
{
    public record FooCreated(MessageId MessageId, string Name) : DomainMessage(MessageId);

    public record ThingDone(MessageId MessageId, string Name, DateTime TimeStampUtc) : DomainMessage(MessageId);
}