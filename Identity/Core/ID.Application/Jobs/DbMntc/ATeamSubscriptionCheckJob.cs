using ID.Application.Jobs.Abstractions;
using System.ComponentModel;

namespace ID.Application.Jobs.DbMntc;

public abstract class ATeamSubscriptionCheckJob() : AMyIdJobHandler("TEAM_SUBSCRIPTIONS_CHECK_JOB")
{
    [DisplayName("MyId - Check Expired Subscriptions")]
    public abstract Task HandleAsync(CancellationToken cancellationToken);
}
