//using DbUp.Builder;
//using ID.Jobs.Quartz.Persistence.MigrationNotifications;
//using Moq;
//using System.Runtime.Serialization;

//namespace ID.Jobs.Quartz.Tests;

//public class QuartzDbMigratorTests
//{
//    private readonly IOptions<QuartzConfig> _options = Options.Create(new QuartzConfig(DatabaseType.PostgreSql, "connstr"));

//    [Fact]
//    public async Task MigrateAsync_When_PerformUpgrade_Fails_Throws()
//    {
//        var mockMigrator = new Mock<IDbUpMigrator>();

//        var failResult = new DatabaseUpgradeResult(new List<SqlScript>(), false, new Exception("fail"), null);
//        var engine = new DummyUpgradeEngine(failResult);

//        mockMigrator.Setup(m => m.MigrateAsync(It.IsAny<Dictionary<string, string>>(), It.IsAny<CancellationToken>())).ReturnsAsync(engine);

//        var notifier = new Mock<IMigrationNotifier>();

//        var svc = new QuartzDbMigrator(_options, mockMigrator.Object, new NullLogger<QuartzDbMigrator>(), notifier.Object);

//        await Should.ThrowAsync<InvalidOperationException>(() => svc.MigrateAsync(CancellationToken.None));
//    }

//    //----------------------//

//    [Fact]
//    public async Task MigrateAsync_When_NoScripts_DoesNotNotify()
//    {
//        var mockMigrator = new Mock<IDbUpMigrator>();
//        var okResult = new DatabaseUpgradeResult(new List<SqlScript>(), true, null, null);

//        var engine = new DummyUpgradeEngine(okResult);
//        mockMigrator.Setup(m => m.MigrateAsync(It.IsAny<Dictionary<string, string>>(), It.IsAny<CancellationToken>())).ReturnsAsync(engine);

//        var notifier = new Mock<IMigrationNotifier>();

//        var svc = new QuartzDbMigrator(_options, mockMigrator.Object, new NullLogger<QuartzDbMigrator>(), notifier.Object);

//        await svc.MigrateAsync(CancellationToken.None);

//        notifier.Verify(n => n.NotifySucceededAsync(It.IsAny<CancellationToken>()), Times.Never);
//    }

//    //----------------------//

//    [Fact]
//    public async Task MigrateAsync_When_ScriptsNotify_Succeeds_InvokesNotifier()
//    {
//        var mockMigrator = new Mock<IDbUpMigrator>();
//        var scripts = new List<SqlScript> { new("x", "sql") };
//        var okResult = new DatabaseUpgradeResult(scripts, true, null, null);

//        var engine = new DummyUpgradeEngine(okResult);
//        mockMigrator.Setup(m => m.MigrateAsync(It.IsAny<Dictionary<string, string>>(), It.IsAny<CancellationToken>())).ReturnsAsync(engine);

//        var notifier = new Mock<IMigrationNotifier>();
//        notifier.Setup(n => n.NotifySucceededAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

//        var svc = new QuartzDbMigrator(_options, mockMigrator.Object, new NullLogger<QuartzDbMigrator>(), notifier.Object);

//        await svc.MigrateAsync(CancellationToken.None);

//        notifier.Verify(n => n.NotifySucceededAsync(It.IsAny<CancellationToken>()), Times.Once);
//    }

//    //----------------------//

//    [Fact]
//    public async Task MigrateAsync_When_NotifierThrows_DoesNotThrow()
//    {
//        var mockMigrator = new Mock<IDbUpMigrator>();
//        var scripts = new List<SqlScript> { new("x", "sql") };
//        var okResult = new DatabaseUpgradeResult(scripts, true, null, null);

//        var engine = new DummyUpgradeEngine(okResult);
//        mockMigrator.Setup(m => m.MigrateAsync(It.IsAny<Dictionary<string, string>>(), It.IsAny<CancellationToken>())).ReturnsAsync(engine);

//        var notifier = new Mock<IMigrationNotifier>();
//        notifier.Setup(n => n.NotifySucceededAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("notify fail"));

//        var svc = new QuartzDbMigrator(_options, mockMigrator.Object, new NullLogger<QuartzDbMigrator>(), notifier.Object);

//        await Should.NotThrowAsync(async () => await svc.MigrateAsync(CancellationToken.None));
//    }
//}

////###########################################################//

//// Test helper: concrete UpgradeEngine subclass that returns a supplied DatabaseUpgradeResult.
//internal class DummyUpgradeEngine(DatabaseUpgradeResult result) 
//    : UpgradeEngine((UpgradeConfiguration)FormatterServices.GetUninitializedObject(typeof(UpgradeConfiguration)))
//{
//    public override DatabaseUpgradeResult PerformUpgrade() => result;
//}
