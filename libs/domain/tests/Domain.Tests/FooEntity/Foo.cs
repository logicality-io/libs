namespace Logicality.Domain.FooEntity;

public class Foo(TimeProvider timeProvider) : Entity<Commands.NewFoo, FooLoadSnapshot, FooSaveSnapshot>
{
    private string _name = null!;

    public void Handle(Commands.DoAThing command)
    {
        var id      = MessageId.From(Guid.NewGuid());
        var now     = timeProvider.GetUtcNow().UtcDateTime;
        var message = new Events.ThingDone(id, command.Name, now);
        RecordMessage(message);
    }

    protected override void InitializeNew(Commands.NewFoo command)
    {
        _name = command.Name;
        var id      = MessageId.From(Guid.NewGuid());
        var message = new Events.FooCreated(id, _name);
        RecordMessage(message);
    }

    protected override void Load(FooLoadSnapshot snapshot)
    {
        Version = snapshot.Version;
        _name   = snapshot.Name;
    }

    protected override FooSaveSnapshot GetSnapshot()
    {
        return new FooSaveSnapshot(Version, _name, [], []);
    }
}