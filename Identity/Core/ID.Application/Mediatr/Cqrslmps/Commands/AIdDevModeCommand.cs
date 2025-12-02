using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Mediatr.Cqrslmps.Commands;



//Values will be set in the pipeline. If not Request will short circuit  with NotFound or Unauthorized. So they will not be null in the handler


//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-//

//BasicResult
/// <summary>
/// Only for tagging Mediatr Command requests that require a PrincipalInfoRequest (User information) in DevMode
/// </summary>
/// <typeparam name="TUser">AppUser</typeparam>
public abstract record AIdDevModeCommand<TUser> : APrincipalInfoRequest, IIdDevModeRequest<TUser>
    where TUser : AppUser
{ }


//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-//


//GenResult<TResponse>
/// <summary>
/// Only for tagging Mediatr Command requests that require a PrincipalInfoRequest (User information) in DevMode
/// </summary>
/// <typeparam name="TUser">AppUser</typeparam>
public abstract record AIdDevModeCommand<TUser, TResponse> :
    APrincipalInfoRequest,
    IIdDevModeRequest<TUser>,
    IIdCommand<TResponse>
    where TUser : AppUser
{ }




