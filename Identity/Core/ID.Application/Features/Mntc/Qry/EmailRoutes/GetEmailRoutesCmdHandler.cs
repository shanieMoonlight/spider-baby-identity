using ID.Application.Mediatr.CqrsAbs;
using ID.GlobalSettings.Constants;
using ID.GlobalSettings.JWT;
using MyResults;

namespace ID.Application.Features.Mntc.Qry.EmailRoutes;
public class GetEmailRoutesCmdHandler : IIdCommandHandler<GetEmailRoutesCmd, EmailRoutesDto>
{

    public Task<GenResult<EmailRoutesDto>> Handle(GetEmailRoutesCmd request, CancellationToken cancellationToken)
    {

        // Create and populate the EmailRoutesDto with values from IdDomainConstants.EmailRoutes
        var emailRoutesDto = new EmailRoutesDto
        {
            ConfirmEmail = IdGlobalConstants.EmailRoutes.ConfirmEmail,
            ConfirmEmailWithPassword = IdGlobalConstants.EmailRoutes.ConfirmEmailWithPassword,
            //ConfirmEmailSpr = IdDomainConstants.EmailRoutes.ConfirmEmailSpr,
            //ConfirmEmailGuest = IdDomainConstants.EmailRoutes.ConfirmEmailGuest,
            RouteParams = new EmailRoutesDto.Params
            {
                UserId = IdGlobalConstants.EmailRoutes.Params.UserId,
                ConfirmationToken = IdGlobalConstants.EmailRoutes.Params.ConfirmationToken
            }
        };

        // Return successful result with the populated DTO
        return Task.FromResult(GenResult<EmailRoutesDto>.Success(emailRoutesDto));
    }

}//Cls

