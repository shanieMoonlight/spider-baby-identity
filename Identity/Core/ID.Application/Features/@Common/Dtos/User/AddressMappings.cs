using ClArch.ValueObjects;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.AppUsers.ValueObjects;

namespace ID.Application.Features.Common.Dtos.User;
internal static class AddressMappings
{

    //----------------------------//

    public static IdentityAddress? ToMdl(this IdentityAddressDto? dto)
    {
        return dto is null
            ? null
            : IdentityAddress.Create(
                AddressLine.Create(dto.Line1),
                AddressLine.Create(dto.Line2),
                AddressLineNullable.Create(dto.Line3),
                AddressLineNullable.Create(dto.Line4),
                AddressLineNullable.Create(dto.Line5),
                AreaCodeNullable.Create(dto.AreaCode),
                ShortNotesNullable.Create(dto.Notes)
            );
    }

    //----------------------------//

    public static IdentityAddressDto? ToDto(this IdentityAddress? mdl) =>
        mdl is null
            ? null
            : new IdentityAddressDto(mdl);

    //----------------------------//

}//Cls