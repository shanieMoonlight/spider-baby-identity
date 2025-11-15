namespace ID.Jobs.Quartz;
internal class QuartzConstants
{
    public const string HandlerTypeKey = "HandlerType";
    public const string MethodNameKey = "MethodName";
    public const string JobGroup = "MyIdJobs";
    public const string Scheduler = "MyIdQuartzScheduler";


    public static class Db
    {
        public const string Schema = "myid_qtz";
        public const string TablePrefix = "QRTZ_";
        public static class MigrationsJournalTable
        {
            public static class Sql
            {
                public const string NAME = "SchemaVersions";
                public static class Columns
                {
                    public const string PRIMARY = "ID";
                    public const string ScriptName = "ScriptName";
                    public const string AppliedAt = "AppliedAt";
                }
            }

            public static class Postgres
            {
                public const string NAME = "schema-versions";
                public static class Columns
                {
                    public const string PRIMARY = "id";
                    public const string ScriptName = "script_name";
                    public const string AppliedAt = "applied_at";
                }
            }
        }
    }
}
