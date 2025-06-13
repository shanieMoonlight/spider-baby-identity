using ClArch.ValueObjects;
using ID.Domain.Entities.Teams;

namespace ID.Application.Features.Teams.Cmd.Dvcs;

public static class DeviceMappings
{
    //------------------------------------//

    public static TeamDevice Update(this TeamDevice model, DeviceDto dto)
    {

        model.Update(
            Name.Create(dto.Name),
            DescriptionNullable.Create(dto.Description)
        );

        return model;

    }

    //------------------------------------//

    public static DeviceDto ToDto(this TeamDevice mdl) => new(mdl);

    //------------------------------------//

}//Cls


