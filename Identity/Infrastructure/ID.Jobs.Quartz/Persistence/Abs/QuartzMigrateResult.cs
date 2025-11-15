namespace ID.Jobs.Quartz.Persistence.Abs;

internal sealed class QuartzMigrateResult
{
    public bool Succeeded { get; }
    public IReadOnlyList<string> AppliedScripts { get; }
    public IReadOnlyList<string> SkippedScripts { get; }
    public string? ErrorMessage { get; }
    public Exception? Exception { get; }

    //----------------------//


    private QuartzMigrateResult(
        bool success,
        IReadOnlyList<string> appliedScripts,
        IReadOnlyList<string> skippedScripts,
        string? errorMessage = null,
        Exception? exception = null)
    {
        Succeeded = success;
        AppliedScripts = appliedScripts;
        SkippedScripts = skippedScripts;
        ErrorMessage = errorMessage;
        Exception = exception;
    }

    //----------------------//

    public static QuartzMigrateResult Success(IReadOnlyList<string> applied, IReadOnlyList<string> skipped) =>
        new(true, applied, skipped, null, null);

    public static QuartzMigrateResult Failure(string message, Exception? ex = null) =>
        new(false, [], [], message, ex);

}//Cls
