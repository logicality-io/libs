namespace Logicality.EventSourcing.Domain;

public class EventSourcedEntityEntry
{
    public EventSourcedEntityEntry(StreamName stream, int expectedVersion, EventSourcedEntity entity)
    {
        Stream          = stream;
        ExpectedVersion = expectedVersion;
        Entity          = entity ?? throw new ArgumentNullException(nameof(entity));
    }

    public StreamName Stream { get; }

    public int ExpectedVersion { get; }
        
    public EventSourcedEntity Entity { get; }
}