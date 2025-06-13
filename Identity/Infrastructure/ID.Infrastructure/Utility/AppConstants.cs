namespace ID.Infrastructure.Utility;
internal class IdInfrastructureConstants
{
   public static class Jobs
    {
        public const string Server = "MyIdServer";
        public const string Schema = "MyIdJobs";
        public const string DashboardPath = "/myid-hangfire";
        public const string DashboardTitle = "MyId Jobs Dashboard";
        public const string BackToAppPath = "/";
        public const string DI_StorageKey = "MyIdDiStorageKey";


        public static class Queues
        {
            public const string Default = "myid-default";
            public const string Priority = "myid-priority";
            public static readonly string[] All = [Default, Priority];
        }

    }
}
