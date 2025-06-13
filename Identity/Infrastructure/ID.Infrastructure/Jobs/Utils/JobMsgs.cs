namespace ID.Infrastructure.Jobs.Utils;
internal class JobMsgs
{
    internal static string MissingConfigJobId(string configName) =>
        $"Invalid {configName} IConfiguration: Missing JobId";

    internal static string MissingConfigKey(string configName, string key) =>
        $"Invalid {configName} IConfiguration: Missing {key}";
}
