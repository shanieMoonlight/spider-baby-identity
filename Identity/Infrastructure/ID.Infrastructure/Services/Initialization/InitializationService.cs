using ID.Application.AppAbs.Setup;
using ID.Application.Jobs.Abstractions;

namespace ID.Infrastructure.Services.Initialization;
public class InitializationService(
    IUserAndRoleDataInitializer initializer, 
    IIdMigrateService migrator,
    IJobsDbMigrator jobsMigrator
    ) : IIdentityInitializationService
{
    /// <summary>
    /// Migrates Db and sets up initial data/users
    /// </summary>
    /// <returns>SuperLeaderEmail if noe is supplie will create default and return it</returns>
    public async Task<string> InitializeEverythingAsync(
        string superLeaderPassword,
        string? superLeaderEmail,
        CancellationToken cancellationToken)
    {
        await MigrateAsync(cancellationToken);
        return await initializer.SeedDataAsync(superLeaderPassword, superLeaderEmail);
    }



    public async Task MigrateAsync(CancellationToken cancellationToken)
    {
        await migrator.MigrateAsync();
        await jobsMigrator.MigrateAsync(cancellationToken);
    }

    public async Task SeedDataAsync(string superLeaderPassword) =>
       await initializer.SeedDataAsync(superLeaderPassword);



    public async Task<bool> IsAlreadyInitializedAsync() =>
       await initializer.IsAlreadyInitializedAsync();

}//Cls
