namespace Logicality.Domain.FooEntity;

public class FooFactory : EntityFactory<Foo, Commands.NewFoo, FooLoadSnapshot, FooSaveSnapshot>;