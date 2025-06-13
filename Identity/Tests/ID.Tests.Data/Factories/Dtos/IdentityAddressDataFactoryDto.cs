using ID.Application.Features.Common.Dtos.User;
using ID.Domain.Entities.AppUsers.ValueObjects;
using MassTransit;
using TestingHelpers;

namespace ID.Tests.Data.Factories.Dtos;

public static class IdentityAddressDataFactoryDto
{

    //- - - - - - - - - - - - - - - - - - //

    public static IdentityAddressDto Create(
        Guid appUserId,
        Guid? id = null,
        string? line1 = null,
        string? line2 = null,
        string? line3 = null,
        string? line4 = null,
        string? line5 = null,
        string? eirCode = null,
        string? administratorUsername = null,
        string? administratorId = null
        )
    {

        line1 ??= $"{RandomStringGenerator.Generate(20)}{id}";
        line2 ??= $"{RandomStringGenerator.Generate(20)}{id}";
        line3 ??= $"{RandomStringGenerator.Generate(20)}{id}";
        line4 ??= $"{RandomStringGenerator.Generate(20)}{id}";
        line5 ??= $"{RandomStringGenerator.Generate(20)}{id}";
        eirCode ??= $"{RandomStringGenerator.Generate(AreaCodeNullable.MaxLength)}{id}";

        id ??= NewId.NextSequentialGuid();
        administratorUsername ??= $"{RandomStringGenerator.Generate(20)}{id}";
        administratorId ??= $"{RandomStringGenerator.Generate(20)}{id}";

        var paramaters = new[]
           {
                new PropertyAssignment(nameof(IdentityAddressDto.Line1),  () => line1 ),
                new PropertyAssignment(nameof(IdentityAddressDto.Line2),  () => line2 ),
                new PropertyAssignment(nameof(IdentityAddressDto.Line3),  () => line3 ),
                new PropertyAssignment(nameof(IdentityAddressDto.Line4),  () => line4 ),
                new PropertyAssignment(nameof(IdentityAddressDto.Line5),  () => line5 ),
                new PropertyAssignment(nameof(IdentityAddressDto.AreaCode),  () => eirCode ),
                new PropertyAssignment(nameof(IdentityAddressDto.AppUserId),  () => appUserId ),
                //new PropertyAssignment(nameof(IdentityAddressDto.Id),  () => id ),
                //new PropertyAssignment(nameof(IdentityAddressDto.AdministratorUsername),  () => administratorUsername ),
                //new PropertyAssignment(nameof(IdentityAddressDto.AdministratorId),  () => administratorId )
        };


        return ConstructorInvoker.CreateNoParamsInstance<IdentityAddressDto>(paramaters);
    }

    //------------------------------------//

}//Cls

