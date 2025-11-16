namespace ID.Email.Base.Cache;

internal class TemplateCacheOptions
{
    public int SlidingExpirationMins { get; set; } = 600; // default 600 minutes. 10 hours. This is long because templates rarely change.
}
