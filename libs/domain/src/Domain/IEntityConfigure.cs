namespace Logicality.Domain;

internal interface IEntityConfigure<TInitializeNewCommand, TSnapShotForLoad>
    where TInitializeNewCommand : DomainCommand
    where TSnapShotForLoad : class
{
    void InitializeNew(TInitializeNewCommand command);

    void Load(TSnapShotForLoad snapShot);
}