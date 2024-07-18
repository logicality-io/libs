namespace Logicality.Domain;

public interface IEntityFactory<TEntity>
{
    void SetServices(IServiceProvider services);
}

public interface IEntityFactory<TEntity, TInitializeNew, TSnapshotForLoad, TSnapshotForSave> : IEntityFactory<TEntity>
    where TEntity : Entity<TInitializeNew, TSnapshotForLoad, TSnapshotForSave>
    where TInitializeNew : DomainCommand
    where TSnapshotForLoad : class
    where TSnapshotForSave : class
{
    TEntity InitializeNew(TInitializeNew newCommand);

    TEntity Load(TSnapshotForLoad snapshot);
}