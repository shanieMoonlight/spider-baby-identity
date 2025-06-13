using ID.Application.AppAbs.Setup;

namespace ID.Infrastructure.Services.Initialization;
public class InitializationService(IUserAndRoleDataInitializer initializer, IIdMigrateService migrator) : IIdentityInitializationService
{
    /// <summary>
    /// Migrates Db and sets up initial data/users
    /// </summary>
    /// <returns>SuperLeaderEmail if noe is supplie will create default and return it</returns>
    public async Task<string> InitializeEverythingAsync(
        string superLeaderPassword,
        string? superLeaderEmail)
    {
        await migrator.MigrateAsync();
        return await initializer.SeedDataAsync(superLeaderPassword, superLeaderEmail);
    }



    public Task MigrateAsync() => migrator.MigrateAsync();



    public async Task SeedDataAsync(string superLeaderPassword) =>
       await initializer.SeedDataAsync(superLeaderPassword);



    public async Task<bool> IsAlreadyInitializedAsync() =>
       await initializer.IsAlreadyInitializedAsync();

}//Cls
