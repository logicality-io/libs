namespace Logicality.Domain.FooEntity;

public record FooSaveSnapshot(int Version, string Name, List<string> ItemsAdded, List<string> ItemsRemoved);