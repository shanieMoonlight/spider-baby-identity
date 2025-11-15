using System.Reflection;

namespace ID.Jobs.Quartz.Persistence.Abs;

internal sealed record QuartzSqlScript(string Name, string Contents); //: SqlScript(name, sql) { }

internal interface IEmbeddedScriptLoader
{
    IReadOnlyList<QuartzSqlScript> LoadEmbeddedSqlScripts(Assembly assembly, string namespacePrefix, IDictionary<string, string> variables);
}