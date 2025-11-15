//using DbUp;
//using DbUp.Engine;
//using ID.Jobs.Quartz.AppImps;
//using ID.Jobs.Quartz.Persistence.Abs;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using System.Reflection;

//namespace ID.Jobs.Quartz.Persistence.DbUp.SqlServer;

//internal class DbUpSqlServerMigrator(
//    IOptions<QuartzConfig> _configProvider, 
//    IEmbeddedScriptLoader _embeddedScriptLoader, 
//    ILogger<QuartzDbMigrator> _logger) : IDbUpMigrator
//{
//    private readonly QuartzConfig _config = _configProvider.Value;

//    //----------------------//

//    public Task<UpgradeEngine> MigrateAsync(Dictionary<string, string> variables, CancellationToken cancellationToken)
//    {
//        string connectionString = _config.ConnectionString;

//        Assembly assembly = IdJobsQrzAssemblyReference.Assembly;

//        //EnsureDatabase.For.SqlDatabase(connectionString);

//        const string nsPrefix = "ID.Jobs.Quartz.Persistence.DbUp.SqlServer.Migrations.";

//        var scripts = _embeddedScriptLoader.LoadEmbeddedSqlScripts(assembly, nsPrefix, variables);

//           var builder = DeployChanges.To
//            .SqlDatabase(connectionString)
//            .JournalToSqlTable(QuartzConstants.Db.JournalSchema, QuartzConstants.Db.JournalTable)
//            .WithScripts(scripts)
//            .LogTo(_logger);

//        var upgrader =  builder.Build();
//        return Task.FromResult(upgrader);
//    }


//}//Cls