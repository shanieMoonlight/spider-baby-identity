using Microsoft.Extensions.Primitives;

namespace ID.Email.Base.Cache;

internal class TemplateCacheInvalidator
{
    private CancellationTokenSource _cts = new();

    public IChangeToken GetChangeToken() => new CancellationChangeToken(_cts.Token);

    public void InvalidateAll()
    {
        var old = Interlocked.Exchange(ref _cts, new CancellationTokenSource());
        try { old.Cancel(); } catch { }
    }
}
