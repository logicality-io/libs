using ValueOf;

namespace ExampleDomain.Profile;

public sealed class ProfileId : ValueOf<string, ProfileId>
{
    public const int MaxLength = 50;

    protected override void Validate()
    {
        if (string.IsNullOrWhiteSpace(Value))
        {
            throw new ArgumentException("Profile Id cannot be null or empty");
        }

        if (Value.Length < 1 || Value.Length >= MaxLength)
        {
            throw new ArgumentOutOfRangeException(
                nameof(Value),
                $"Profile Id length must be greater than 0 and smaller than {MaxLength} characters")
            {
                Data =
                {
                    { "Value", Value }
                },
            };
        }
    }
}