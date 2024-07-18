namespace Logicality.Domain;

public interface IDeterministicGuidFactory
{
    public Guid Create(byte[] input);
}