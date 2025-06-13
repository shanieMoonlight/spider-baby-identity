using ClArch.ValueObjects;
using ID.Application.Customers.Dtos.User;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.AppUsers.OAuth;
using ID.Domain.Entities.AppUsers.ValueObjects;
using MyResults;

namespace ID.Application.Customers.Abstractions;
public interface IIdCustomerRegistrationService
{
    //-------------------------------------// 

    Task<GenResult<AppUser>> Register_NoPwd_Async(
        RegisterCustomer_NoPwdDto dto,
        CancellationToken cancellationToken);

    //- - - - - - - - - - - - - - - - - - //   

    Task<GenResult<AppUser>> Register_NoPwd_Async(
        EmailAddress email,
        UsernameNullable username,
        PhoneNullable phone,
        FirstNameNullable firstName,
        LastNameNullable lastName,
        TeamPositionNullable position,
        Guid? subscriptionPlanId = null,
        CancellationToken cancellationToken = default);

    //-------------------------------------//

    Task<GenResult<AppUser>> RegisterAsync(
        RegisterCustomerDto dto,
        CancellationToken cancellationToken);

    //- - - - - - - - - - - - - - - - - - //   

    Task<GenResult<AppUser>> RegisterAsync(
        EmailAddress email,
        UsernameNullable username,
        PhoneNullable phone,
        FirstNameNullable firstName,
        LastNameNullable lastName,
        Password password,
        ConfirmPassword confirmPassword,
        TeamPositionNullable position,
        Guid? subscriptionPlanId,
        CancellationToken cancellationToken);

    //- - - - - - - - - - - - - - - - - - //   

    Task<GenResult<AppUser>> RegisterOAuthAsync(
        EmailAddress email,
        UsernameNullable username,
        PhoneNullable phone,
        FirstNameNullable firstName,
        LastNameNullable lastName,
        TeamPositionNullable position,
        OAuthInfo oAuthInfo,
        Guid? subscriptionPlanId,
        CancellationToken cancellationToken);

    //-------------------------------------//

}//Cls