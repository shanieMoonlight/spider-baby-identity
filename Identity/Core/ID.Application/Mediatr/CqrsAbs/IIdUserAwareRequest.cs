using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;

namespace ID.Application.Mediatr.CqrsAbs;



public interface IIdUserAwareRequest<TUser> : IIdPrincipalInfoRequest where TUser : AppUser
{
    /// <summary>
    /// Current User/Principal/Client
    /// </summary>
    public TUser PrincipalUser { get; set; }

}//int

//- - - - - - - - - - - - - - - - - -//

public interface IIdUserAndTeamAwareRequest<TUser> : IIdUserAwareRequest<TUser> where TUser : AppUser
{
    /// <summary>
    /// The Team containing current User/Principal/Client
    /// </summary>
    public Team PrincipalTeam { get; protected set; }

    //- - - - - - - - - - - - //

    IIdUserAndTeamAwareRequest<TUser> SetTeam(Team PrincipalTeam);

}//int