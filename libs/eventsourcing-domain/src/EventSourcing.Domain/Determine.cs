namespace Logicality.EventSourcing.Domain;

public static class Determine
{
    private static readonly NamespaceBasedGuidGenerator Generator = new NamespaceBasedGuidGenerator(
        new Guid("10a8d5dd-c836-434e-bc42-b62e559171b5")
    );
        
    public static Guid NextId(Guid previous, int index)
    {
        var input = new List<byte>(16 + 4);
        input.AddRange(previous.ToByteArray());
        input.AddRange(BitConverter.GetBytes(index));
        return Generator.Create(input.ToArray());
    }
}