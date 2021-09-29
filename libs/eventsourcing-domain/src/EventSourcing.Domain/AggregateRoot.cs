namespace Logicality.EventSourcing.Domain
{
    public class AggregateRoot : EventSourcedEntity
    {
        public int? Version { get; private set; }

        public void SetVersion(int version) => Version = version;
    }
}
