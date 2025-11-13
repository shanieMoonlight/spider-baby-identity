namespace ID.Application.AppAbs.Setup;

public interface IIdentityInitializationService
{
    /// <summary>
    /// Migrates Db and sets up initial data/users.
    /// </summary>
    /// <returns>SuperLeaderEmail</returns>
    Task<string> InitializeEverythingAsync(string superLeaderPassword, string? superLeaderEmail, CancellationToken cancellationToken);

    Task<bool> IsAlreadyInitializedAsync();

    /// <summary>
    /// Migrates Db
    /// </summary>
    Task MigrateAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Set up initial users/data
    /// </summary>
    Task SeedDataAsync(string superLeaderPassword);

}//Cls