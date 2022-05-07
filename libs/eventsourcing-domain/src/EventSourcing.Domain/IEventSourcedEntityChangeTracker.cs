namespace Logicality.EventSourcing.Domain;

public interface IEventSourcedEntityChangeTracker
{   
    bool TryGetEntry(StreamName stream, out EventSourcedEntityEntry entry);
        
    void TrackEntry(EventSourcedEntityEntry entry);
}