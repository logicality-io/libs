namespace Logicality.Domain.FooEntity;

public record FooLoadSnapshot(int Version, string Name, List<string> Items);