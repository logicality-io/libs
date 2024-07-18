namespace Logicality.Domain;

public abstract class
    Entity<TInitializeNew, TSnapshotForLoad, TSnapshotForSave> : IEntityConfigure<TInitializeNew, TSnapshotForLoad>,
    IEntity
    where TInitializeNew : DomainCommand
    where TSnapshotForLoad : class
    where TSnapshotForSave : class
{
    private readonly MessageRecorder _messageRecorder = new();

    protected int Version { get; set; }

    void IEntityConfigure<TInitializeNew, TSnapshotForLoad>.InitializeNew(TInitializeNew command)
    {
        InitializeNew(command);
    }

    void IEntityConfigure<TInitializeNew, TSnapshotForLoad>.Load(TSnapshotForLoad snapshot)
    {
        Load(snapshot);
    }

    protected abstract void InitializeNew(TInitializeNew command);

    protected abstract void Load(TSnapshotForLoad command);

    protected abstract TSnapshotForSave GetSnapshot();

    public IReadOnlyCollection<DomainMessage> GetMessages() => _messageRecorder.RecordedMessages;

    protected void RecordMessage(DomainMessage message)
    {
        _messageRecorder.Record(message);
    }
}