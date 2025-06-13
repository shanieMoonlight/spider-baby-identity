using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;

namespace ID.Application.Mediatr.Cqrslmps.Queries;


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.


//Values will be set in the pipeline. If not Request will short circuit  with NotFound or Unauthorized. So they will not be null in the handler


//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-//
//BasicResult


/// <summary>
/// Class for creating requests that contain the Pincipal info and the actual AppUser and their Team with their Full Team. 
/// Will cause a NotFoundResult to be returned in the Pipeline if the user or Team was not found.
/// So User will NOT be null in the Handler
/// </summary>
/// <typeparam name="TUser">Type of AppUser</typeparam>
public abstract record AIdUserAndTeamAwareQuery<TUser> : AIdUserAwareQuery<TUser>, IIdUserAndTeamAwareRequest<TUser> where TUser : AppUser
{
    /// <summary>
    /// The Team containing current User/Principal/Client
    /// This will be set by TeamAwarePipelineBehavior if Team was not found the pipleine will exit with a NotFoundResult and will not reach the handler
    /// </summary>  
    public Team PrincipalTeam { get; set; }

    //- - - - - - - - - - - - //

    public IIdUserAndTeamAwareRequest<TUser> SetTeam(Team principalTeam)
    {
        PrincipalTeam = principalTeam;
        IsLeader = principalTeam?.LeaderId == PrincipalUser?.Id;
        return this;
    }
}


//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-//
//GenResult


/// <summary>
/// Class for creating requests that contain the Pincipal info and the actual AppUser and their Team with their Full Team. 
/// Will cause a NotFoundResult to be returned in the Pipeline if the user or their Team was not found.
/// So User will NOT be null in the Handler
/// </summary>
/// <typeparam name="TUser">Type of AppUser</typeparam>
public abstract record AIdUserAndTeamAwareQuery<TUser, TResponse> : AIdUserAwareQuery<TUser, TResponse>, IIdUserAndTeamAwareRequest<TUser> where TUser : AppUser
{
    /// <summary>
    /// The Team containing current User/Principal/Client
    /// This will be set by TeamAwarePipelineBehavior if Team was not found the pipleine will exit with a NotFoundResult and will not reach the handler
    /// </summary>
    public Team PrincipalTeam { get; set; }

    //- - - - - - - - - - - - //

    public IIdUserAndTeamAwareRequest<TUser> SetTeam(Team principalTeam)
    {
        PrincipalTeam = principalTeam;
        IsLeader = principalTeam?.LeaderId == PrincipalUserId;
        return this;
    }
}



//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-//


#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
