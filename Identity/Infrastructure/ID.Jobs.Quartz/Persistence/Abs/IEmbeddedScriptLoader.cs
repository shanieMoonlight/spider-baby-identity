using DbUp.Engine;
using System.Reflection;

namespace ID.Jobs.Quartz.Persistence.Abs;
internal interface IEmbeddedScriptLoader
{
    IReadOnlyList<SqlScript> LoadEmbeddedSqlScripts(Assembly assembly, string namespacePrefix, IDictionary<string, string> variables);
}