using ID.Domain.Entities.AppUsers;

namespace ID.Application.Mediatr.CqrsAbs;


/// <summary>
/// Interface for requests that require user information in development mode.
/// </summary>
/// <typeparam name="TUser"></typeparam>
public interface IIdDevModeRequest<TUser> : IIdPrincipalInfoRequest where TUser : AppUser { }