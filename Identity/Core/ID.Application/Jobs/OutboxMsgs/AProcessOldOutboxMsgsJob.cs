using ID.Application.Jobs.Abstractions;
using System.ComponentModel;

namespace ID.Application.Jobs.OutboxMsgs;

public abstract class AProcess_Old_MyIdOutboxMsgs() : AMyIdJobHandler("OLD_OUTBOX_MSGS")
{
    [DisplayName("MyId - Remove Old Outbox Msgs")]
    public abstract Task HandleAsync();
}


