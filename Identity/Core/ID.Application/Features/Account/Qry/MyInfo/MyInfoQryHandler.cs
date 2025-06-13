using ID.Application.Dtos.User;
using ID.Application.Features.Common.Dtos.User;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;
using MyResults;

namespace ID.Application.Features.Account.Qry.MyInfo;
public class MyInfoQryHandler() : IIdQueryHandler<MyInfoQry, AppUserDto>
{
    public Task<GenResult<AppUserDto>> Handle(MyInfoQry request, CancellationToken cancellationToken)
    {
        AppUser user = request.PrincipalUser!; // UserAwarePipelineBehavior ensures this is not null

        return Task.FromResult(GenResult<AppUserDto>.Success(user.ToDto()));

    }

}//Cls
