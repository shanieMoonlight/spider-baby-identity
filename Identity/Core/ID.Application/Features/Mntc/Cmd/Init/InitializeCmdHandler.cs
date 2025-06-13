using ID.Application.AppAbs.Setup;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Utility.Messages;
using MyResults;

namespace ID.Application.Features.Mntc.Cmd.Init;
public class InitializeCmdHandler(IIdentityInitializationService idInitialization) : IIdCommandHandler<InitializeCmd>
{

    public async Task<BasicResult> Handle(InitializeCmd request, CancellationToken cancellationToken)
    {
        var password = request.Dto.Password;
        var email = request.Dto.Email;
        var superLeaderEmail = await idInitialization.InitializeEverythingAsync(password, email);

        return BasicResult.Success(IDMsgs.Info.INITIALIZED(superLeaderEmail));

    }

}//Cls

