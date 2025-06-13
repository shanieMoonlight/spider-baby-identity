using ID.Application.Jobs.Abstractions;
using System.ComponentModel;

namespace ID.Application.Jobs.DbMntc;

public abstract class AOldRefreshTokensJob() : AMyIdJobHandler("OLD_REFRESH_TOKENS_JOB")
{
    [DisplayName("MyId - Remove any expired refresh Tokens")]
    public abstract Task HandleAsync(CancellationToken cancellationToken);
}

