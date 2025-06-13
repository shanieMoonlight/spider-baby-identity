using ID.Application.Mediatr.CqrsAbs;
using ID.Application.Mediatr.Cqrslmps;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Mediatr.Cqrslmps.Commands;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.


//Values will be set in the pipeline. If not Request will short circuit  with NotFound or Unauthorized. So they will not be null in the handler


//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-//

/// <summary>
/// Class for creating requests that contain the Pincipal info and the actual AppUser. 
/// Will cause a NotFoundResult to be returned in the Pipeline if the user was not found.
/// So User will NOT be null in the Handler
/// </summary>
/// <typeparam name="TUser">Type of AppUser</typeparam>
public abstract record AIdUserAwareCommand<TUser> :
    APrincipalInfoRequest,
    IIdUserAwareRequest<TUser>,
    IIdCommand
    where TUser : AppUser
{
    /// <summary>
    /// <inheritdoc cref="IIdUserAwareRequest.PrincipalUser"/>
    /// </summary>
    public TUser PrincipalUser { get; set; }
}


//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-//
//GenResult


/// <summary>
/// Class for creating requests that contain the Pincipal info and the actual AppUser. 
/// Will cause a NotFoundResult to be returned in the Pipeline if the user was not found.
/// So User will NOT be null in the Handler
/// </summary>
/// <typeparam name="TUser">Type of AppUser</typeparam>
public abstract record AIdUserAwareCommand<TUser, TResponse>
    : APrincipalInfoRequest,
    IIdUserAwareRequest<TUser>,
    IIdCommand<TResponse>
    where TUser : AppUser
{
    /// <summary>
    /// <inheritdoc cref="IIdUserAwareRequest.PrincipalUser"/>
    /// </summary>
    public TUser PrincipalUser { get; set; }
}


//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-//


#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
