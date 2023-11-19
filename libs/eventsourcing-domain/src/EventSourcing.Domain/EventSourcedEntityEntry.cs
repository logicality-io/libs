namespace Logicality.EventSourcing.Domain;

public class EventSourcedEntityEntry(StreamName stream, int expectedVersion, EventSourcedEntity entity)
{
    public StreamName Stream { get; } = stream;

    public int ExpectedVersion { get; } = expectedVersion;

    public EventSourcedEntity Entity { get; } = entity ?? throw new ArgumentNullException(nameof(entity));
}