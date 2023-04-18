using Logicality.EventSourcing.Domain.Testing;
using Xunit;

namespace ExampleDomain.Tests.Profile
{
    public class ProfileTests
    {
        [Fact]
        public void Test1()
        {
            return new Scenario()
                .GivenNone()
                .When(this.user.As10YearsOld(this.when).FromSupportedCountryAndAcceptableNationality().SignsUp())
                .Then(this.stream,
                    new ClientSignUpRejected
                    {
                        TenantId = this.user.TenantId,
                        ProfileId = this.user.ProfileId,
                        FirstName = this.user.FirstName,
                        LastName = this.user.LastName,
                        Country = this.user.Country,
                        Nationality = this.user.Nationality,
                        RejectedReasons = new[] { RejectedReason.DoesNotMeetMinimumAgeRequirement },
                        EmailAddress = this.user.EmailAddress,
                        BirthDate = this.when.AddYears(-10),
                        When = this.when,
                    },
                    new MarketingConsentGiven { TenantId = this.user.TenantId, ProfileId = this.user.ProfileId, When = this.when },
                    new TermsAndConditionsGotAccepted { TenantId = this.user.TenantId, ProfileId = this.user.ProfileId, When = this.when },
                    new NotificationConsentGiven { TenantId = this.user.TenantId, ProfileId = this.user.ProfileId, When = this.when },
                    new ProfileDocument
                    {
                        TenantId = this.user.TenantId,
                        ProfileId = this.user.ProfileId,
                        FirstName = this.user.FirstName,
                        LastName = this.user.LastName,
                        Country = this.user.Country,
                        MarketingConsentGiven = true,
                        RestrictedCountry = false,
                        Nationality = this.user.Nationality,
                        NotificationConsentGiven = true,
                        RestrictedCitizen = false,
                        RestrictedNationality = false,
                        ClientStatus = ClientStatus.Blocked,
                        EmailAddress = this.user.EmailAddress,
                        BirthDate = this.when.AddYears(-10),
                        RejectedReasons = new[]
                        {
                            RejectedReason.DoesNotMeetMinimumAgeRequirement,
                        },
                        BlockedClientActions = new Messages.BlockableClientAction[0],
                        AddressVerificationStatus = new AddressVerificationStatus
                        {
                            SwissPostStatus = this.user.Country != "CHE" && this.user.Country != "LIE" ? SwissPostStatus.NotApplicable : SwissPostStatus.NotStarted,
                        },
                        Blocked = false,
                    })
                .Expect(new RejectedSignUpResult(new Dictionary<string, string[]>
                {
                    { "DoesNotMeetMinimumAgeRequirement", new[] { "User does not meet minimum age requirement" } },
                }))
                .AssertAsync(this.runner);
        }
    }
}