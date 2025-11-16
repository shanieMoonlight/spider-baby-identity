using System.Reflection;
using ID.Email.Base.LocalAbs;
using Microsoft.Extensions.Hosting;
using MyResults;

namespace ID.Email.Base.LocalImps;
internal class TemplateLoader(IHostEnvironment env) : ITemplateLoader
{
    public async Task<GenResult<string>> LoadAsync(string templatePath)
    {
        // Normalize separators (works on Windows/Linux)
        var normalizedTemplatePath = templatePath
            .Replace('\\', Path.DirectorySeparatorChar)
            .Replace('/', Path.DirectorySeparatorChar)
            .TrimStart(Path.DirectorySeparatorChar);


        // 1) Try app content root (published files / operator overrides)
        var contentRoot = env.ContentRootPath;
        var filePath = Path.Combine(contentRoot, normalizedTemplatePath);
        if (File.Exists(filePath))
        {
            await using var fs = File.OpenRead(filePath);
            using var sr = new StreamReader(fs);
            var template = await sr.ReadToEndAsync();
            return GenResult<string>.Success(template);
        }


        // 2) Dev / bin fallback (helps when running from build output)
        var buildDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var binFallback = Path.Combine(buildDir!, normalizedTemplatePath);
        if (File.Exists(binFallback))
        {
            await using var fs = File.OpenRead(binFallback);
            using var sr = new StreamReader(fs);
            var template = await sr.ReadToEndAsync();
            return GenResult<string>.Success(template);
        }


        // 3) Embedded resource fallback
        var asm = Assembly.GetExecutingAssembly();
        var manifestNames = asm.GetManifestResourceNames();

        // candidate strategies: dotted path, namespace + dotted path, file name suffix
        var candidate1 = normalizedTemplatePath.Replace(Path.DirectorySeparatorChar, '.'); // e.g. Assets.html-templates.EmailConfirmation.IdEmailConfirmationCustomer.html
        var @namespace = typeof(TemplateLoader).Namespace ?? asm.GetName().Name!;
        var candidate2 = $"{@namespace}.{candidate1}";

        var resourceName = manifestNames.FirstOrDefault(n =>
            string.Equals(n, candidate2, StringComparison.OrdinalIgnoreCase)
            || string.Equals(n, candidate1, StringComparison.OrdinalIgnoreCase)
            || n.EndsWith(candidate1, StringComparison.OrdinalIgnoreCase)
            || n.EndsWith(Path.GetFileName(normalizedTemplatePath), StringComparison.OrdinalIgnoreCase));

        if (resourceName is not null)
        {
            await using var rs = asm.GetManifestResourceStream(resourceName)
                ?? throw new FileNotFoundException(resourceName);
            using var sr = new StreamReader(rs);
            var template = await sr.ReadToEndAsync();
            return GenResult<string>.Success(template);
        }

        return GenResult<string>.NotFoundResult($"Template not found. Available embedded resources: {string.Join(',', manifestNames.Take(20))}");
    }

}//Cls
