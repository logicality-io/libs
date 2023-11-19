using Logicality.EventSourcing.Domain;

namespace ExampleDomain.Profile;

public class Profile : EventSourcedEntity
{
    public static readonly Func<Profile> Factory = () => new Profile();

    public ProfileId? Id { get; private set; }

    public Profile()
    {
        On<UserRegistered>(e =>
        {
            Id = ProfileId.From(e.ProfileId);
        });
    }

    public static Profile Register(
        ProfileId profileId,
        string    firstName,
        string    lastName,
        string    emailAddress)
    {
        var profile = new Profile();
        profile.Apply(new UserRegistered(profileId.Value, firstName, lastName, emailAddress));
        return profile;
    }
}