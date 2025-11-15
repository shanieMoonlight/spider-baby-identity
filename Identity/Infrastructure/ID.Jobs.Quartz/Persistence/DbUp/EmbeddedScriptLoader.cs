using ID.Jobs.Quartz.Persistence.Abs;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace ID.Jobs.Quartz.Persistence.DbUp;

internal class EmbeddedScriptLoader(ILogger<EmbeddedScriptLoader> _logger) : IEmbeddedScriptLoader
{
    public IReadOnlyList<QuartzSqlScript> LoadEmbeddedSqlScripts(
        Assembly assembly, string namespacePrefix, IDictionary<string, string> variables)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        ArgumentException.ThrowIfNullOrWhiteSpace(namespacePrefix);

        var resourceNames = assembly.GetManifestResourceNames()
            .Where(n => n.StartsWith(namespacePrefix, StringComparison.OrdinalIgnoreCase) && n.EndsWith(".sql.template", StringComparison.OrdinalIgnoreCase))
            .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (resourceNames.Length == 0)
            throw new InvalidOperationException($"No embedded SQL migrations found under '{namespacePrefix}' in assembly {assembly.FullName}.");

        var scripts = new List<QuartzSqlScript>(resourceNames.Length);

        foreach (var res in resourceNames)
        {
            using var stream = assembly.GetManifestResourceStream(res)
                ?? throw new FileNotFoundException(res);
            using var reader = new StreamReader(stream);
            var sql = reader.ReadToEnd();

            if (variables != null && variables.TryGetValue("schema", out var schema))
            {
                // support both token variants
                sql = sql.Replace("${schema}", schema, StringComparison.Ordinal)
                         .Replace("$schema$", schema, StringComparison.Ordinal);
            }

            var scriptName = res[namespacePrefix.Length..];
            _logger?.LogDebug("Prepared script: {ScriptName}", scriptName);
            scripts.Add(new QuartzSqlScript(scriptName, sql));
        }

        return scripts;
    }

}//Cls
