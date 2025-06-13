using ID.Application.Mediatr;
using ID.Application.Mediatr.Behaviours;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace ID.Application.Tests.Mediatr;
public class IdMediatrSetupExtensionsTests
{

    public static TheoryData<Type[]> BehavioursInCorrectOrder()
    {
        return
        [
            [
                typeof(IdPrincipalPipelineBehavior<,>),
                typeof(IdUserAwarePipelineBehavior<,>),
                typeof(IdTeamAwarePipelineBehavior<,>)
            ]
        ];
    }
    [Theory]
    [MemberData(nameof(BehavioursInCorrectOrder))]
    public void AddIdApplication_Registers_UserAndTeamInfo_BehaviorsInCorrectOrder2(Type[] behaviourTypesInOrder)
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();
        var assembly = typeof(IdApplicationAssemblyReference).Assembly;

        // Act
        services = services.AddMyIdMediatr(assembly);

        // Assert
        Assert.NotNull(services);

        var plServices = services.Where(s => s.ServiceType == typeof(IPipelineBehavior<,>));
        var svcIdxList = new List<ServiceIdx>();
        foreach (var svc in plServices)
        {
            svcIdxList.Add(new ServiceIdx(svc, services.IndexOf(svc)));
        }

        var filteredSvcIdxList = svcIdxList.Where(s => behaviourTypesInOrder.Contains(s.ImplementationType)).ToList();

        //Ensure that type is registered
        foreach (var svcType in behaviourTypesInOrder)
        {
            filteredSvcIdxList.Any(si => si.ImplementationType == svcType).ShouldBeTrue();
        }

        // Ensure the order matches
        for (int i = 0; i < behaviourTypesInOrder.Length - 1; i++)
        {
            var currentType = behaviourTypesInOrder[i];
            var nextType = behaviourTypesInOrder[i + 1];

            var currentIdx = filteredSvcIdxList.First(si => si.ImplementationType == currentType).Idx;
            var nextIdx = filteredSvcIdxList.First(si => si.ImplementationType == nextType).Idx;

            currentIdx.ShouldBeLessThan(nextIdx);
        }
    }

    //------------------------------------//

    [Fact]
    public void AddIdApplication_Registers_UserAndTeamInfo_BehaviorsInCorrectOrder()
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();

        var assembly = typeof(IdApplicationAssemblyReference).Assembly;


        // Act
        services = services.AddMyIdMediatr(assembly);

        // Assert
        Assert.NotNull(services);

        var plServices = services.Where(s => s.ServiceType == typeof(IPipelineBehavior<,>));
        var svcIdxList = new List<ServiceIdx>();
        foreach (var svc in plServices)
        {
            svcIdxList.Add(new ServiceIdx(svc, services.IndexOf(svc)));
        }


        var principalBehaviorsIdx = svcIdxList.FirstOrDefault(s => s.ImplementationType == typeof(IdPrincipalPipelineBehavior<,>));
        var userAwareBehaviorsIdx = svcIdxList.FirstOrDefault(s => s.ImplementationType == typeof(IdUserAwarePipelineBehavior<,>));
        var teamAwareBehaviorsIdx = svcIdxList.FirstOrDefault(s => s.ImplementationType == typeof(IdTeamAwarePipelineBehavior<,>));

        principalBehaviorsIdx.ShouldNotBeNull();
        userAwareBehaviorsIdx.ShouldNotBeNull();
        teamAwareBehaviorsIdx.ShouldNotBeNull();

        principalBehaviorsIdx!.Idx.ShouldBeLessThan(userAwareBehaviorsIdx!.Idx);
        principalBehaviorsIdx!.Idx.ShouldBeLessThan(teamAwareBehaviorsIdx!.Idx);

        userAwareBehaviorsIdx?.Idx.ShouldBeLessThan(teamAwareBehaviorsIdx!.Idx);

    }

    //------------------------------------//

}//Cls





//=============================================================================//



internal record ServiceIdx(ServiceDescriptor Service, int Idx)
{
    public Type? ImplementationType => Service.ImplementationType;

}


//=============================================================================//

