using ClArch.ValueObjects;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.AppUsers.ValueObjects;
using ID.Domain.Entities.Teams;
using ID.Domain.Models;
using MassTransit;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using TestingHelpers;

namespace ID.Tests.Data.Factories;

public static class AppUserDataFactory
{

    public static AppUser AnyUser = AppUser.Create(
         TeamDataFactory.AnyTeam,
         EmailAddress.Create("anyone@anywhere.com"),
         UsernameNullable.Create("anyone"),
         PhoneNullable.Create("066 666 666 66"),
         FirstNameNullable.Create("Clarke"),
         LastNameNullable.Create("Kent"),
         TeamPositionNullable.Create(2)
     );

    //------------------------------------//

    public static List<AppUser> CreateMany(int count = 20)
    {
        return [.. IdGenerator.GetGuidIdsList(count).Select(id => Create(id))];
    }

    //- - - - - - - - - - - - - - - - - - //

    public static AppUser Create(
        Guid? teamId = null,
        Guid? id = null,
        string? firstName = null,
        string? lastName = null,
        string? userName = null,
        string? email = null,
        string? phoneNumber = null,
        string? password = null,
        bool? emailConfirmed = null,
        string? tkn = null,
        string? twoFactorKey = null,
        string? normalizedUserName = null,
        string? normalizedEmail = null,
        string? securityStamp = null,
        string? concurrencyStamp = null,
        bool? phoneNumberConfirmed = null,
        bool? twoFactorEnabled = null,
        Team? team = null,
        IdentityAddress? identityAddress = null,
        TwoFactorProvider? twoFactorProvider = null,
        int? teamPosition = 1,
        DateTime? tknModifiedDate = null,
        string? passwordHash = null,
        bool? lockoutEnabled = null,
        int? accessFailedCount = null,
        string? administratorUsername = null,
        string? administratorId = null
        )
    {
        PasswordHasher<AppUser> hasher = new();
        passwordHash ??= Convert.ToBase64String(HashPasswordV2(password ?? "Pa$$0Rd!", RandomNumberGenerator.Create()));

        administratorUsername ??= $"{RandomStringGenerator.Generate(20)}{id}";
        administratorId ??= $"{RandomStringGenerator.Generate(20)}{id}";
        firstName ??= $"{MyRandomDataGenerator.FirstName()}{id}";
        lastName ??= $"{MyRandomDataGenerator.LastName()}{id}";
        tkn ??= $"{RandomStringGenerator.Generate(20)}{id}";
        twoFactorKey ??= $"{RandomStringGenerator.Generate(20)}{id}";
        //freindlyName ??= $"{RandomStringGenerator.Generate(20)}{id}";
        id ??= NewId.NextSequentialGuid();
        userName ??= $"{MyRandomDataGenerator.Username()}{id}";
        normalizedUserName ??= $"{RandomStringGenerator.Generate(20)}{id}";
        email ??= $"{MyRandomDataGenerator.Email().ToLower()}";
        normalizedEmail ??= $"{RandomStringGenerator.Generate(20)}{id}";
        emailConfirmed ??= false;
        securityStamp ??= $"{RandomStringGenerator.Generate(20)}{id}";
        concurrencyStamp ??= $"{RandomStringGenerator.Generate(20)}{id}";
        phoneNumber ??= $"{MyRandomDataGenerator.Phone()}";
        phoneNumberConfirmed ??= false;
        twoFactorEnabled ??= false;
        lockoutEnabled ??= false;
        accessFailedCount ??= 0;
        twoFactorProvider ??= TwoFactorProvider.Email;
        teamPosition ??= 1;

        var paramaters = new[]
           {
               new PropertyAssignment(nameof(AppUser.AdministratorUsername),  () => administratorUsername ),
                new PropertyAssignment(nameof(AppUser.AdministratorId),  () => administratorId ),
                new PropertyAssignment(nameof(AppUser.FirstName),  () => firstName ),
                new PropertyAssignment(nameof(AppUser.LastName),  () => lastName ),
                new PropertyAssignment(nameof(AppUser.TeamId),  () => teamId ?? team?.Id ),
                new PropertyAssignment(nameof(AppUser.Team),  () => team ),
                new PropertyAssignment(nameof(AppUser.Tkn),  () => tkn ),
                new PropertyAssignment(nameof(AppUser.TwoFactorKey),  () => twoFactorKey ),
                //new PropertyAssignment(nameof(AppUser.FriendlyName),  () => freindlyName ),
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
                new PropertyAssignment(nameof(AppUser.LockoutEnabled),  () => lockoutEnabled ),
                new PropertyAssignment(nameof(AppUser.AccessFailedCount),  () => accessFailedCount ), 
                new PropertyAssignment(nameof(AppUser.TwoFactorEnabled),  () => twoFactorEnabled ),
                new PropertyAssignment(nameof(AppUser.TeamPosition),  () => teamPosition ),
                new PropertyAssignment(nameof(AppUser.Address),  () => identityAddress ),
                new PropertyAssignment(nameof(AppUser.TknModifiedDate),  () => tknModifiedDate )
                //new PropertyAssignment(nameof(AppUser.TwoFactorProvider),  () => twoFactorProvider ),
        };


        var user =  NonPublicConstructorInvoker.CreateNoParamsInstance<AppUser>(paramaters);
        user.Update2FactorProvider(twoFactorProvider ??= TwoFactorProvider.Email); 
        return user;
    }

    //------------------------------------//

    public static AppUser CreateNoTeam(
      Guid? id = null,
      string? firstName = null,
      string? lastName = null,
      string? userName = null,
      string? email = null,
      string? phoneNumber = null,
      string? password = null,
      bool? emailConfirmed = null,
      string? tkn = null,
      string? twoFactorKey = null,
      string? normalizedUserName = null,
      string? normalizedEmail = null,
      string? securityStamp = null,
      string? concurrencyStamp = null,
      bool? phoneNumberConfirmed = null,
      bool? twoFactorEnabled = null,
      bool? lockoutEnabled = null,
      int? accessFailedCount = null,
      string? passwordHash = null,
      string? freindlyName = null,
      string? administratorUsername = null,
      string? administratorId = null
      )
    {
        PasswordHasher<AppUser> hasher = new();
        passwordHash ??= Convert.ToBase64String(HashPasswordV2(password ?? "Pa$$0Rd!", RandomNumberGenerator.Create()));

        administratorUsername ??= $"{RandomStringGenerator.Generate(20)}{id}";
        administratorId ??= $"{RandomStringGenerator.Generate(20)}{id}";
        firstName ??= $"{MyRandomDataGenerator.FirstName()}{id}";
        lastName ??= $"{MyRandomDataGenerator.LastName()}{id}";
        tkn ??= $"{RandomStringGenerator.Generate(20)}{id}";
        twoFactorKey ??= $"{RandomStringGenerator.Generate(20)}{id}";
        //freindlyName ??= $"{RandomStringGenerator.Generate(20)}{id}";
        id ??= NewId.NextSequentialGuid();
        userName ??= $"{MyRandomDataGenerator.Username()}{id}";
        normalizedUserName ??= $"{RandomStringGenerator.Generate(20)}{id}";
        email ??= $"{MyRandomDataGenerator.Email()}{id}";
        normalizedEmail ??= $"{RandomStringGenerator.Generate(20)}{id}";
        emailConfirmed ??= false;
        securityStamp ??= $"{RandomStringGenerator.Generate(20)}{id}";
        concurrencyStamp ??= $"{RandomStringGenerator.Generate(20)}{id}";
        phoneNumber ??= $"{MyRandomDataGenerator.Phone()}{id}";
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
                new PropertyAssignment(nameof(AppUser.Tkn),  () => tkn ),
                new PropertyAssignment(nameof(AppUser.TwoFactorKey),  () => twoFactorKey ),
                //new PropertyAssignment(nameof(AppUser.FriendlyName),  () => freindlyName ),
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


        return ConstructorInvoker.CreateNoParamsInstance<AppUser>(paramaters);
    }

    //------------------------------------//


    private static byte[] HashPasswordV2(string password, RandomNumberGenerator rng)
    {
        const KeyDerivationPrf Pbkdf2Prf = KeyDerivationPrf.HMACSHA1; // default for Rfc2898DeriveBytes
        const int Pbkdf2IterCount = 1000; // default for Rfc2898DeriveBytes
        const int Pbkdf2SubkeyLength = 256 / 8; // 256 bits
        const int SaltSize = 128 / 8; // 128 bits

        // Produce a version 2 (see comment above) text hash.
        byte[] salt = new byte[SaltSize];
        rng.GetBytes(salt);
        byte[] subkey = KeyDerivation.Pbkdf2(password, salt, Pbkdf2Prf, Pbkdf2IterCount, Pbkdf2SubkeyLength);

        var outputBytes = new byte[1 + SaltSize + Pbkdf2SubkeyLength];
        outputBytes[0] = 0x00; // format marker
        Buffer.BlockCopy(salt, 0, outputBytes, 1, SaltSize);
        Buffer.BlockCopy(subkey, 0, outputBytes, 1 + SaltSize, Pbkdf2SubkeyLength);
        return outputBytes;
    }

    //------------------------------------//
}//Cls

