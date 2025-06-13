using ID.Tests.Data.Factories;
using Newtonsoft.Json;
using Shouldly;
using System.IdentityModel.Tokens.Jwt;
using ID.Domain.Entities.Teams;
using ID.Domain.Claims.Utils;
using ID.Application.JWT.Subscriptions;

namespace ID.Application.Tests.Jwt;

public class SubscriptionClaimDataTests
{
    [Fact]
    public void Constructor_Should_Set_Properties_Correctly()
    {
        // Arrange
        var device = DeviceDataFactory.Create();
        var subPlan = SubscriptionPlanDataFactory.Create();
        var teamSubscription = SubscriptionDataFactory.Create(plan: subPlan, devices: [device]);

        // Act
        var claimData = SubscriptionClaimData.Create(teamSubscription, device.UniqueId);

        // Assert
        claimData.Trial.ShouldBe((teamSubscription.TrialEndDate > DateTime.Now));
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        claimData.Name.ShouldBe(teamSubscription.SubscriptionPlan.Name.ToLower());
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        claimData.Status.ShouldBe(teamSubscription.SubscriptionStatus.ToString().ToLower());
        claimData.Expiry.ShouldBe(new DateTimeOffset(teamSubscription.EndDate ?? DateTime.MaxValue).ToUnixTimeSeconds().ToString().ToLower());
    }

    //------------------------------------//

    [Fact]
    public void Serialize_Should_Return_Correct_Json()
    {
        // Arrange
        var device = DeviceDataFactory.Create();
        var subPlan = SubscriptionPlanDataFactory.Create();
        var teamSubscription = SubscriptionDataFactory.Create(plan: subPlan, devices: [device]);

        var claimData = SubscriptionClaimData.Create(teamSubscription, device.UniqueId);
        var expectedJson = JsonConvert.SerializeObject(claimData, new JsonSerializerSettings
        {
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
        });

        // Act
        var json = claimData.Serialize();

        // Assert
        json.ShouldBe(expectedJson);
    }

    //------------------------------------//
}

public class SubscriptionClaimDataExtensionsTests
{
    [Fact]
    public void ToClaim_Should_Return_Valid_Claim()
    {
        // Arrange
        var device = DeviceDataFactory.Create();
        var subPlan = SubscriptionPlanDataFactory.Create();
        var teamSubscription = SubscriptionDataFactory.Create(plan: subPlan, devices: [device]);

        // Act
        var claim = teamSubscription.ToClaim(device.UniqueId);

        // Assert
        claim.Type.ShouldBe(MyIdClaimTypes.TEAM_SUBSCRIPTIONS);
        claim.ValueType.ShouldBe(JsonClaimValueTypes.Json);
        SubscriptionClaimData? deserializedData = JsonConvert.DeserializeObject<SubscriptionClaimData>(claim.Value, new JsonSerializerSettings
        {
            ContractResolver = new SubscriptionClaimDataContractResolver()
        });
        deserializedData.ShouldNotBeNull();
        deserializedData.Name.ShouldBe(teamSubscription.SubscriptionPlan?.Name.ToLower());
    }

    //------------------------------------//

    [Fact]
    public void ToClaims_Should_Return_List_Of_Claims()
    {
        // Arrange
        var device = DeviceDataFactory.Create();
        var subPlan1 = SubscriptionPlanDataFactory.Create();
        var subPlan2 = SubscriptionPlanDataFactory.Create();
        var subscriptions = new List<TeamSubscription>
        {
           SubscriptionDataFactory.Create(plan:subPlan1, devices: [device]),
           SubscriptionDataFactory.Create(plan:subPlan2)
        };

        // Act
        var claims = subscriptions.ToClaims(device.UniqueId);

        // Assert
        claims.ShouldNotBeNull();
        claims.Count.ShouldBe(2);
    }

    //------------------------------------//
}
