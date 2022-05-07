using System;

namespace Logicality.EventSourcing.Domain;

public readonly struct StreamName : IEquatable<StreamName>
{
    private readonly string _value;

    public StreamName(string value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    public bool Equals(StreamName other) => string.Equals(_value, other._value, StringComparison.Ordinal);

    public override bool Equals(object obj) => obj is StreamName other && Equals(other);
        
    public override int GetHashCode() => _value == null ? 0 : _value.GetHashCode();

    public override string ToString() => _value ?? "";
}