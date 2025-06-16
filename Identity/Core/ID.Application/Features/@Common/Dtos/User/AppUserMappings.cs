using ID.Application.Features.Common.Dtos.User;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Dtos.User;

public static class AppUserMappings
{

    public static AppUserDto ToDto(this AppUser mdl) => new(mdl);


    //------------------------------------//


    public static IEnumerable<AppUserDto> ToDtos(this IEnumerable<AppUser> teams) =>
        teams.Select(t => t.ToDto());


}//Cls


