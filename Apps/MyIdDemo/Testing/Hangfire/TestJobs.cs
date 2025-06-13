using System.Diagnostics;

namespace MyIdDemo.Testing.Hangfire;

public class TestJobs
{
    public static async Task TestJob1()
    {
        Console.Beep();
        await Task.Delay(180000); // Non-blocking alternative to Thread.Sleep
        Debug.WriteLine("Test Job 1 executed at: " + DateTime.Now);
        Console.WriteLine("Test Job 1 executed at: " + DateTime.Now);
    }

    public static async Task TestJob2()
    {
        Console.Beep();
        await Task.Delay(280000); // Non-blocking alternative to Thread.Sleep
        Debug.WriteLine("Test Job 2 executed at: " + DateTime.Now);
        Console.WriteLine("Test Job 2 executed at: " + DateTime.Now);
    }


    public static async Task TestJob3()
    {
        Console.Beep();
        await Task.Delay(380000); // Non-blocking alternative to Thread.Sleep
        Debug.WriteLine("Test Job 3 executed at: " + DateTime.Now);
        Console.WriteLine("Test Job 3 executed at: " + DateTime.Now);
    }

}
