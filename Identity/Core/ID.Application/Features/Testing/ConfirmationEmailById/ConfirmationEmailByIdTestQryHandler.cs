using ID.Application.AppAbs.ApplicationServices;
using ID.Application.AppAbs.ApplicationServices.User;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Utility.Messages;
using MyResults;

namespace ID.Application.Features.Testing.ConfirmationEmailById;
internal class ConfirmationEmailByIdTestQryHandler(IFindUserService<AppUser> findUserService, IEmailConfirmationBus emailConfirmationBus)
    : IIdQueryHandler<ConfirmationEmailByIdTestQry>
{

    public async Task<BasicResult> Handle(ConfirmationEmailByIdTestQry request, CancellationToken cancellationToken)
    {
        var id = request.Id;

        //Check if user exists
        var user = await findUserService.FindUserWithTeamDetailsAsync(id);
        if (user == null)
            return BasicResult.NotFoundResult(IDMsgs.Error.Authorization.INVALID_LOGIN);

        await emailConfirmationBus.GenerateTokenAndPublishEventAsync(user, user.Team!, cancellationToken);
        return BasicResult.PreconditionRequiredResult(IDMsgs.Error.Email.EMAIL_NOT_CONFIRMED(user.Email ?? "no-email"));

    }

}//Cls

