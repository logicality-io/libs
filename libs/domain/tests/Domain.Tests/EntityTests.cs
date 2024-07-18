using Logicality.Domain.FooEntity;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Logicality.Domain;

public class EntityTests
{
    [Fact]
    public void Create_entity_and_handle_command()
    {
        var serviceCollection = new ServiceCollection()
            .AddSingleton(TimeProvider.System);
        serviceCollection
            .AddDomain()
            .AddEntity<Foo, FooFactory>();
        var services = serviceCollection.BuildServiceProvider();

        var fooFactory = services.GetRequiredService<FooFactory>();

        var newFoo = new Commands.NewFoo(CommandId.From(Guid.NewGuid()), "Damo");
        var foo    = fooFactory.InitializeNew(newFoo);
        
        var commandId = CommandId.From(Guid.NewGuid());
        var command   = new Commands.DoAThing(commandId, "Example");
        foo.Handle(command);

        var messages = foo.GetMessages();
        messages.Count.ShouldBe(2);
    }
}