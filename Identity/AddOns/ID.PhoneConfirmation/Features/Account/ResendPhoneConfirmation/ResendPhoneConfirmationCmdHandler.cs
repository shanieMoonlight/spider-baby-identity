using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Domain.Utility.Messages;
using ID.PhoneConfirmation.Events.Integration.Bus;
using MyResults;

namespace ID.PhoneConfirmation.Features.Account.ResendPhoneConfirmation;
public class ResendPhoneConfirmationCmdHandler(IPhoneConfirmationBus bus) : IIdCommandHandler<ResendPhoneConfirmationCmd>
{

    //------------------------------------//

    public async Task<BasicResult> Handle(ResendPhoneConfirmationCmd request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var currentTeam = request.PrincipalTeam;
        var phoneUser = FindUser(currentTeam, dto);

        if (phoneUser is null)
            return BasicResult.NotFoundResult(IDMsgs.Error.NotFound<AppUser>(dto.Username ?? dto.Email ?? dto.UserId.ToString()));

        if (phoneUser.PhoneNumberConfirmed)
            return BasicResult.Success(IDMsgs.Info.Phone.PHONE_ALREADY_CONFIRMED(phoneUser.PhoneNumber));


        await bus.GenerateTokenAndPublishEventAsync(phoneUser, currentTeam, cancellationToken);


        return BasicResult.Success(IDMsgs.Info.Phone.PHONE_CONFIRMED(phoneUser.PhoneNumber!));
    }

    //------------------------------------//


    private static AppUser? FindUser(Team currentTeam, ResendPhoneConfirmationDto dto)
    {
        AppUser? user = null;
        user = currentTeam.Members.FirstOrDefault(u => u.Id == dto.UserId);

        user ??= currentTeam.Members.FirstOrDefault(u => u.Email == dto.Email);
        user ??= currentTeam.Members.FirstOrDefault(u => u.UserName == dto.Username);

        return user;
    }

    //------------------------------------//


}//Cls
