using MyResults;
using ID.Application.AppAbs.Messaging;
using ID.Domain.Utility.Messages;

namespace ID.Infrastructure.Services.Messaging;

public class IdFallbackMsgService : IIdSmsService, IIdWhatsAppService
{

    public Task<BasicResult> SendMsgAsync(string number, string message)
       => Task.FromResult(BasicResult.Failure(IDMsgs.Error.Teams.NO_MSG_PROVIDER_SET));


}//Cls