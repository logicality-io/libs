using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Logicality.Domain;

public static class DomainServiceCollectionExtensions
{
    public static IDomainServiceCollection AddDomain(this IServiceCollection serviceCollection)
    {
        var @namespace = Guid.Parse("E8D2E2F1-778E-4E6F-A81C-C72A6E5DE6D8");
        serviceCollection.TryAddScoped<MessageRecorder>();
        serviceCollection.TryAddSingleton<IDeterministicGuidFactory>(_ = new NamespaceBasedDeterministicGuidFactory(@namespace));
        return new DomainServiceCollection(serviceCollection);
    }

    private class DomainServiceCollection(IServiceCollection serviceCollection) : IDomainServiceCollection
    {
        public IDomainServiceCollection AddEntity<TEntity, TEntityFactory>() 
            where TEntity : class, IEntity 
            where TEntityFactory : class, IEntityFactory<TEntity>, new()
        {
            serviceCollection.AddTransient<TEntity>();
            serviceCollection.AddTransient(services =>
            {
                var entityFactory = new TEntityFactory();
                entityFactory.SetServices(services);
                return entityFactory;
            });
            return this;
        }
    }
}

public class EntityFactory<TEntity, TInitializeNew, TSnapshotForLoad, TSnapshotForSave>
    : IEntityFactory<TEntity, TInitializeNew, TSnapshotForLoad, TSnapshotForSave>
    where TEntity : Entity<TInitializeNew, TSnapshotForLoad, TSnapshotForSave>
    where TInitializeNew : DomainCommand
    where TSnapshotForLoad : class
    where TSnapshotForSave : class
{
    private IServiceProvider _services = null!;

    void IEntityFactory<TEntity>.SetServices(IServiceProvider services)
    {
        _services = services;
    }

    public TEntity InitializeNew(TInitializeNew command)
    {
        var entity          = _services.GetRequiredService<TEntity>();
        var entityConfigure = (IEntityConfigure<TInitializeNew, TSnapshotForLoad>)entity;
        entityConfigure.InitializeNew(command);
        return entity;
    }

    public TEntity Load(TSnapshotForLoad snapshot)
    {
        var entity          = _services.GetRequiredService<TEntity>();
        var entityConfigure = (IEntityConfigure<TInitializeNew, TSnapshotForLoad>)entity;
        entityConfigure.Load(snapshot);
        return entity;
    }
}