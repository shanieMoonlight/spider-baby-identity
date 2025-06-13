using ID.Application.Mediatr.Validation;
using Microsoft.AspNetCore.Hosting;

namespace ID.Application.Features.Mntc.Cmd.Migrate;
public class MigrateCmdValidator(IWebHostEnvironment env) : AMntcMinimumOrDevValidator<MigrateCmd>(env)
{
}//Cls

