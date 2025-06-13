using ClArch.ValueObjects;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Features.MemberMgmt.Cmd.UpdateSelf;
internal static class UpdateSelfMappings
{


    public static AppUser Update(this AppUser model, UpdateSelfDto dto) =>
        model.Update(
            EmailAddress.Create(dto.Email),
            UsernameNullable.Create(dto.Username),
            PhoneNullable.Create(dto.PhoneNumber),
            FirstNameNullable.Create(dto.FirstName),
            LastNameNullable.Create(dto.LastName),
            dto.TwoFactorProvider,
            dto.TwoFactorEnabled
        );

}//Cls