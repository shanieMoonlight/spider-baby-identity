using ID.Application.Models;

namespace ID.Jobs.Quartz;
internal record QuartzConfig(DatabaseType DatabaseType, string ConnectionString);