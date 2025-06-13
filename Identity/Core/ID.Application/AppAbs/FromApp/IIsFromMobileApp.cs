namespace ID.Application.AppAbs.FromApp;

/// <summary>
/// Used in FromAppMiddleware
/// </summary>
public interface IIsFromMobileApp
{
    public bool IsFromApp { get; set; }
}
