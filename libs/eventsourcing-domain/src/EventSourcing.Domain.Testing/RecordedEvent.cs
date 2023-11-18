namespace Logicality.EventSourcing.Domain.Testing;

public class RecordedEvent
{
    public StreamName Stream { get; }

    public object Message { get; }
        
    public RecordedEvent(StreamName stream, object message)
    {
        Stream  = stream;
        Message = message ?? throw new ArgumentNullException(nameof(message));
    }
}