using ID.Application.Features.Common.Dtos.User;
using ID.Application.Mediatr.Cqrslmps.Queries;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Features.Account.Qry.MyInfo;

/// <summary>
/// Query to retrieve current user's profile information.
/// </summary>
public record MyInfoQry() : AIdUserAwareQuery<AppUser, AppUserDto>;



