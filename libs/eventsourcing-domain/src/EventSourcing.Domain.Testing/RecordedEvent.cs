namespace Logicality.EventSourcing.Domain.Testing;

public class RecordedEvent(StreamName stream, object message)
{
    public StreamName Stream { get; } = stream;

    public object Message { get; } = message ?? throw new ArgumentNullException(nameof(message));
}