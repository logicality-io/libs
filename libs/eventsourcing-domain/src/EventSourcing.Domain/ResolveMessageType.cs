namespace Logicality.EventSourcing.Domain;

public static class ResolveMessageType
{
    private static void WithSupersededTypes(this IDictionary<string, Type> cache)
    {
    }
        
    public static MessageTypeResolver WhenEqualToTypeName(IEnumerable<Type> types)
    {
        if (types == null)
        {
            throw new ArgumentNullException(nameof(types));
        }

        var cache = types.ToDictionary(type => type.Name);
        cache.WithSupersededTypes();

        return name =>
        {
            if (!cache.TryGetValue(name, out var type))
            {
                throw new InvalidOperationException($"The type for message named {name} could not be found.");
            }

            return type;
        };
    }
}