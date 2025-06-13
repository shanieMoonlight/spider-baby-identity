using ID.Application.Features.Common.Dtos.User;
using ID.Domain.Entities.AppUsers;
using MassTransit;
using TestingHelpers;

namespace ID.Tests.Data.Factories.Dtos;

public static class AppUserDtoDataFactory
{

    public static List<AppUserDto> CreateMany(int count = 20)
    {
        return [.. IdGenerator.GetGuidIdsList(count).Select(id => Create(id))];
    }

    //- - - - - - - - - - - - - - - - - - //

    public static AppUserDto Create(
          Guid? id = null,
        string? administratorUsername = null,
        string? administratorId = null,
        string? firstName = null,
        string? lastName = null,
        Guid? teamId = null,
        string? tkn = null,
        string? twoFactorKey = null,
        string? freindlyName = null,
        string? userName = null,
        string? normalizedUserName = null,
        string? email = null,
        string? normalizedEmail = null,
        bool? emailConfirmed = null,
        string? passwordHash = null,
        string? securityStamp = null,
        string? concurrencyStamp = null,
        string? phoneNumber = null,
        bool? phoneNumberConfirmed = null,
        bool? twoFactorEnabled = null,
        bool? lockoutEnabled = null,
        int? accessFailedCount = null
        )
    {

        administratorUsername ??= $"{RandomStringGenerator.Generate(20)}{id}";
        administratorId ??= $"{RandomStringGenerator.Generate(20)}{id}";
        firstName ??= $"{RandomStringGenerator.Generate(20)}{id}";
        lastName ??= $"{RandomStringGenerator.Generate(20)}{id}";
        teamId ??= NewId.NextSequentialGuid();
        tkn ??= $"{RandomStringGenerator.Generate(20)}{id}";
        twoFactorKey ??= $"{RandomStringGenerator.Generate(20)}{id}";
        freindlyName ??= $"{RandomStringGenerator.Generate(20)}{id}";
        id ??= NewId.NextSequentialGuid();
        userName ??= $"{RandomStringGenerator.Generate(20)}{id}";
        normalizedUserName ??= $"{RandomStringGenerator.Generate(20)}{id}";
        email ??= $"{RandomStringGenerator.Generate(20)}{id}";
        normalizedEmail ??= $"{RandomStringGenerator.Generate(20)}{id}";
        emailConfirmed ??= false;
        passwordHash ??= $"{RandomStringGenerator.Generate(20)}{id}";
        securityStamp ??= $"{RandomStringGenerator.Generate(20)}{id}";
        concurrencyStamp ??= $"{RandomStringGenerator.Generate(20)}{id}";
        phoneNumber ??= $"{RandomStringGenerator.Generate(20)}{id}";
        phoneNumberConfirmed ??= false;
        twoFactorEnabled ??= false;
        lockoutEnabled ??= false;
        accessFailedCount ??= 0;

        var paramaters = new[]
           {
               new PropertyAssignment(nameof(AppUser.AdministratorUsername),  () => administratorUsername ),
        new PropertyAssignment(nameof(AppUser.AdministratorId),  () => administratorId ),
        new PropertyAssignment(nameof(AppUser.FirstName),  () => firstName ),
        new PropertyAssignment(nameof(AppUser.LastName),  () => lastName ),
        new PropertyAssignment(nameof(AppUser.TeamId),  () => teamId ),
        new PropertyAssignment(nameof(AppUser.Tkn),  () => tkn ),
        new PropertyAssignment(nameof(AppUser.TwoFactorKey),  () => twoFactorKey ),
        new PropertyAssignment(nameof(AppUser.FriendlyName),  () => freindlyName ),
        new PropertyAssignment(nameof(AppUser.Id),  () => id ),
        new PropertyAssignment(nameof(AppUser.UserName),  () => userName ),
        new PropertyAssignment(nameof(AppUser.NormalizedUserName),  () => normalizedUserName ),
        new PropertyAssignment(nameof(AppUser.Email),  () => email ),
        new PropertyAssignment(nameof(AppUser.NormalizedEmail),  () => normalizedEmail ),
        new PropertyAssignment(nameof(AppUser.EmailConfirmed),  () => emailConfirmed ),
        new PropertyAssignment(nameof(AppUser.PasswordHash),  () => passwordHash ),
        new PropertyAssignment(nameof(AppUser.SecurityStamp),  () => securityStamp ),
        new PropertyAssignment(nameof(AppUser.ConcurrencyStamp),  () => concurrencyStamp ),
        new PropertyAssignment(nameof(AppUser.PhoneNumber),  () => phoneNumber ),
        new PropertyAssignment(nameof(AppUser.PhoneNumberConfirmed),  () => phoneNumberConfirmed ),
        new PropertyAssignment(nameof(AppUser.TwoFactorEnabled),  () => twoFactorEnabled ),
        new PropertyAssignment(nameof(AppUser.LockoutEnabled),  () => lockoutEnabled ),
        new PropertyAssignment(nameof(AppUser.AccessFailedCount),  () => accessFailedCount )
        };


        return ConstructorInvoker.CreateNoParamsInstance<AppUserDto>(paramaters);
    }

    //------------------------------------//

}//Cls

