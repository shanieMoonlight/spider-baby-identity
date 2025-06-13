using ID.Application.Jobs.Abstractions;
using System.ComponentModel;

namespace ID.GlobalSettings.Jobs.DbMntc;

public abstract class ATeamLeaderMntcJob() : AMyIdJobHandler("TEAM_LEADER_MNTC_JOB")
{
    [DisplayName("MyId - Missing team leader job")]
    public abstract Task HandleAsync(CancellationToken cancellationToken);
}

