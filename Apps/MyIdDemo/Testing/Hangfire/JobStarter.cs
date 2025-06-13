using Hangfire;

namespace MyIdDemo.Testing.Hangfire;

public class JobStarter
{

    public static void StartTestJob1()
    {
        // This method will be called by Hangfire to start the job
        RecurringJob.AddOrUpdate("job1", () => TestJobs.TestJob1(), Cron.Daily());

        var enq = BackgroundJob.Enqueue(() => TestJobs.TestJob2());
        var sch = BackgroundJob.Schedule(() => TestJobs.TestJob3(), TimeSpan.FromDays(1));

    }

    //- - - - - - - - - - - - - - - - - //


    public static void StartAllTestJobs()
    {
        StartTestJob1();
    }


}//Cls
