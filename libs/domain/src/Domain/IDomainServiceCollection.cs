namespace Logicality.Domain;

public interface IDomainServiceCollection
{
    IDomainServiceCollection AddEntity<TEntity, TEntityFactory>()
        where TEntity : class, IEntity
        where TEntityFactory : class, IEntityFactory<TEntity>, new();
}