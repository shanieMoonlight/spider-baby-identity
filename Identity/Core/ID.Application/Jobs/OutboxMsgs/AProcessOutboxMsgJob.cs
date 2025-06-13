using ID.Application.Jobs.Abstractions;
using System.ComponentModel;

namespace ID.Application.Jobs.OutboxMsgs;

public abstract class AProcessMyIdOutboxMsgJob() : AMyIdJobHandler("OUTBOX_HANDLER")
{
    [DisplayName("MyId - Process Outbox Msgs")]
    public abstract Task HandleAsync();
}


