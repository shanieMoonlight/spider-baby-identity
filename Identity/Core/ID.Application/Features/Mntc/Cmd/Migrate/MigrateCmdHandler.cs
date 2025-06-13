using ID.Application.AppAbs.Setup;
using ID.Application.Mediatr.CqrsAbs;
using MyResults;

namespace ID.Application.Features.Mntc.Cmd.Migrate;
public class MigrateCmdHandler(IIdentityInitializationService idInitialization) : IIdCommandHandler<MigrateCmd>
{

    public async Task<BasicResult> Handle(MigrateCmd request, CancellationToken cancellationToken)
    {
        await idInitialization.MigrateAsync();
        return BasicResult.Success("Migrated!");

    }

}//Cls

