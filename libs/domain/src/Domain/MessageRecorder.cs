namespace Logicality.Domain;

public class MessageRecorder
{
    private readonly List<DomainMessage>                _recorded = new();
    public           IReadOnlyCollection<DomainMessage> RecordedMessages => _recorded;

    public void Record(DomainMessage message) =>
        _recorded.Add(message);

    public void Clear() =>
        _recorded.Clear();
}