namespace TestingHelpers;

public static class RandomDateGenerator
{
    public static DateTime Generate(DateTime? startDate = null, DateTime? endDate = null)
    {
        startDate ??= new DateTime(2020, 1, 1);
        endDate ??= startDate.Value.AddYears(1);

        var timeSpan = endDate.Value - startDate.Value;
        var rand = new Random();
        var days = rand.Next(0, timeSpan.Days + 1);
        return startDate.Value.AddDays(days);
    }
}