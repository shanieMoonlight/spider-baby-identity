using ClArch.ValueObjects;
using ID.Domain.Entities.SubscriptionPlans;
using ID.Domain.Entities.SubscriptionPlans.ValueObjects;
using ID.Domain.Entities.Teams;
using ID.Domain.Utility.Exceptions;
using ID.Tests.Data.Factories;
using MediatR;
using MyResults;
using Shouldly;

namespace ID.Application.Tests.Mediatr.Pipeline.Exceptions.Helpers;

//- - - - - - - - - - - - - - - - - - //

public class DeviceLimitExceededHandler : IRequestHandler<TestExceptionsRequest, BasicResult>
{
    public Task<BasicResult> Handle(TestExceptionsRequest request, CancellationToken cancellationToken)
    {    //    // Arrange
        var dvcLimit = 6;
        //var msg = "Device limit exceeded";
        var plan = SubscriptionPlan.Create(
            Name.Create("Plan"),
            Description.Create("PlanDescription"),
            Price.Create(666),
            SubscriptionRenewalTypes.Monthly,
            TrialMonths.Create(6),
            DeviceLimit.Create(dvcLimit));
        var team = TeamDataFactory.Create();
        var sub = TeamSubscription.Create(plan, team, Discount.Create(0));
        throw new DeviceLimitExceededException(sub);
    }
}

//- - - - - - - - - - - - - - - - - - //

public class DeviceLimitExceededExceptionTestHelper
{
    internal static TestParamaters Params =>
        new(() => MyContainerProvider.GetContainer<DeviceLimitExceededHandler, DeviceLimitExceededException>(true), Challenge);


    internal static void Challenge(BasicResult response)
    {
        response.BadRequest.ShouldBeTrue();
    }

}

//- - - - - - - - - - - - - - - - - - //


