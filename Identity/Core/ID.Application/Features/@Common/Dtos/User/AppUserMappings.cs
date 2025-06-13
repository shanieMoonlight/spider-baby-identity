using ID.Application.Features.Common.Dtos.User;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Dtos.User;

public static class AppUserMappings
{
    //------------------------------------//

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

    //------------------------------------//

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

    //------------------------------------//

    public static AppUserDto ToDto(this AppUser mdl) => new(mdl);

    //------------------------------------//

    public static IEnumerable<AppUserDto> ToDtos(this IEnumerable<AppUser> teams) =>
        teams.Select(t => t.ToDto());

    //------------------------------------//

}//Cls


