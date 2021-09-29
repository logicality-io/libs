using System;
using System.Collections.Generic;
using System.Linq;

namespace Logicality.EventSourcing.Domain
{
    public static class ResolveMessageName
    {
        public static MessageNameResolver WhenEqualToTypeName(IEnumerable<Type> types)
        {
            if (types == null)
            {
                throw new ArgumentNullException(nameof(types));
            }

            var cache = types.ToDictionary(type => type, type => type.Name);

            return type =>
            {
                if (type == null)
                {
                    throw new ArgumentNullException(nameof(type));
                }

                if (!cache.TryGetValue(type, out var name))
                {
                    throw new InvalidOperationException($"The name for message typed as {type.FullName} could not be found.");
                }

                return name;
            };
        }
    }
}