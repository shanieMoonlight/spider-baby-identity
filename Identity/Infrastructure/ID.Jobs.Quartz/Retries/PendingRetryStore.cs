using System.Collections.Concurrent;

namespace ID.Jobs.Quartz.Retries;

internal sealed class PendingRetryStore
{
    private readonly ConcurrentDictionary<string, PendingRetry> _dict = new();

    public bool TryAdd(string id, PendingRetry item) => _dict.TryAdd(id, item);

    public bool TryRemove(string id, out PendingRetry? item) => _dict.TryRemove(id, out item);

    public bool TryGet(string id, out PendingRetry? item) => _dict.TryGetValue(id, out item);

    // snapshot current items (id + item)
    public KeyValuePair<string, PendingRetry>[] Snapshot() => [.. _dict];
}