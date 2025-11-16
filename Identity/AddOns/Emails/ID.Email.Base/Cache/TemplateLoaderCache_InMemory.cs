using ID.Email.Base.LocalAbs;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyResults;
using System.Security.Cryptography;
using System.Text;

namespace ID.Email.Base.Cache;
internal class TemplateLoaderCache_InMemory(
    ITemplateLoader _originalLoader,
    IMemoryCache _cache,
    IOptions<TemplateCacheOptions> _optionsProvider,
    TemplateCacheInvalidator _invalidator,
    ILogger<TemplateLoaderCache_InMemory> _logger,
    IHostEnvironment _env)
    : ITemplateLoader
{
    private readonly TemplateCacheOptions _settings = _optionsProvider.Value;
    private static readonly string _cacheKeyPrefix = $"{nameof(TemplateLoaderCache_InMemory)}";
    private MemoryCacheEntryOptions DefaultCacheOptions => new()
    {
        SlidingExpiration = TimeSpan.FromMinutes(_settings.SlidingExpirationMins)
    };

    //--------------------------//

    public async Task<GenResult<string>> LoadAsync(string templatePath)
    {
        var key = BuildCacheKey(templatePath);

        if (_cache.TryGetValue(key, out string? cached) && cached is not null)
            return GenResult<string>.Success(cached);

        var result = await _cache.GetOrCreateAsync(key, async entry =>
        {
            entry.SetOptions(DefaultCacheOptions);
            entry.AddExpirationToken(_invalidator.GetChangeToken());

            var res = await _originalLoader.LoadAsync(templatePath);
            if (!res.Succeeded)
                return string.Empty;

            return res.Value ?? string.Empty;
        });

        return string.IsNullOrEmpty(result)
            ? GenResult<string>.NotFoundResult()
            : GenResult<string>.Success(result);
    }

    //--------------------------//

    private string BuildCacheKey(string templatePath)
    {
        // normalize the path to a stable form
        var normalized = templatePath.Replace('\\', '/').Replace('/', '/').TrimStart('/');

        // Try to get last-write time of a published/dev file so on-disk updates invalidate cache automatically
        string? fileStamp = null;
        try
        {
            // content root (published files / operator overrides)
            var contentRoot = _env.ContentRootPath;
            var candidate = Path.Combine(contentRoot, normalized.Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(candidate))
            {
                fileStamp = File.GetLastWriteTimeUtc(candidate).ToString("yyyyMMddHHmmss");
            }
            else
            {
                // bin/build fallback (helps when running from build output)
                var asmBin = Path.GetDirectoryName(IdEmailBaseAssemblyReference.Assembly?.Location ?? string.Empty);
                if (!string.IsNullOrEmpty(asmBin))
                {
                    var binCandidate = Path.Combine(asmBin, normalized.Replace('/', Path.DirectorySeparatorChar));
                    if (File.Exists(binCandidate))
                        fileStamp = File.GetLastWriteTimeUtc(binCandidate).ToString("yyyyMMddHHmmss");
                }
            }
        }
        catch { /* ignore file-system issues */ }

        // assembly stamp as fallback
        var asm = IdEmailBaseAssemblyReference.Assembly ?? _originalLoader.GetType().Assembly;
        var version = asm.GetName().Version?.ToString() ?? string.Empty;
        var asmStamp = string.Empty;
        try
        {
            var loc = asm.Location;
            if (!string.IsNullOrEmpty(loc) && File.Exists(loc))
                asmStamp = File.GetLastWriteTimeUtc(loc).ToString("yyyyMMddHHmmss");
        }
        catch { }

        var stamp = fileStamp ?? asmStamp;
        var fileName = Path.GetFileName(normalized);

        // compute SHA-256 of the normalized path
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(normalized)))[..16]; // 16 hex chars

        return $"{_cacheKeyPrefix}.V-{version}.T-{stamp}.F-{fileName}.{hash}";
    }

}//Cls
