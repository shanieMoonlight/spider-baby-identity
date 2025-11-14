namespace ID.Jobs.Quartz;
internal class QuartzConstants
{
    public const string HandlerTypeKey = "HandlerType";
    public const string MethodNameKey = "MethodName";
    public const string JobGroup = "MyIdJobs";
    public const string Schema = "myid_qtz";
    public const string TablePrefix = "QRTZ_";
    public const string Scheduler = "MyIdQuartzScheduler";


    public static class DbUp
    {
        public const string JournalTable = "SchemaVersions";
        public const string JournalSchema = Schema;
    }
}
