using ID.Domain.Entities.AppUsers;

namespace ID.Application.Customers.Features.Common.Dtos.User;

public static class AppUserMappings
{
    //----------------------//

    //public static AppUser Update(this AppUser model, AppUserDto dto)
    //{

    //    model.Update(
    //        model.Id,
    //        EmailAddress.Create(dto.Email),
    //        UsernameNullable.Create(dto.UserName),
    //        PhoneNullable.Create(dto.PhoneNumber),
    //        FirstNameNullable.Create(dto.FirstName),
    //        LastNameNullable.Create(dto.LastName)
    //    );

    //    return model;

    //}

    //----------------------//

    //public static AppUser ToModel(this AppUserDto dto, NewId teamId)
    //{

    //    return AppUser.Create(
    //        EmailAddress.Create(dto.Email),
    //        UsernameNullable.Create(dto.UserName),
    //        PhoneNullable.Create(dto.PhoneNumber),
    //        FirstNameNullable.Create(dto.FirstName),
    //        LastNameNullable.Create(dto.LastName),
    //        teamId
    //    );

    //}

    //----------------------//

    public static AppUser_Customer_Dto ToCustomerDto(this AppUser appUser) => new()
    {
        Email = appUser.Email ?? "no-email",
        PhoneNumber = appUser.PhoneNumber,
        Id = appUser.Id,
        UserName = appUser.UserName,
        FirstName = appUser.FirstName,
        LastName = appUser.LastName,
        TeamId = appUser.TeamId,
        TeamPosition = appUser.TeamPosition,
        TwoFactorProvider = appUser.TwoFactorProvider,
        TwoFactorEnabled = appUser.TwoFactorEnabled,
        EmailConfirmed = appUser.EmailConfirmed,

    };

    //----------------------//

    public static IEnumerable<AppUser_Customer_Dto> ToCustomerDtos(this IEnumerable<AppUser> teams) =>
        teams.Select(t => t.ToCustomerDto());

    //----------------------//

}//Cls


