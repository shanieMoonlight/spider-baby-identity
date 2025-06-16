using ID.Domain.Models;

namespace ID.Application.AppAbs.ApplicationServices.TwoFactor;



/// <param name="TwoFactorProvider"> How will 2 factor be verified </param>
/// <param name="ExtraInfo"> Info on failure </param>
public record MfaResultData(TwoFactorProvider TwoFactorProvider, string? ExtraInfo = null)
{
    /// <summary>
    /// Info on failure
    /// </summary>
    public string? ExtraInfo { get; private set; } = ExtraInfo;

    public TwoFactorProvider TwoFactorProvider { get; private set; } = TwoFactorProvider;

    public static MfaResultData Create(TwoFactorProvider twoFactorProvider, string? extraInfo = null) =>
        new(twoFactorProvider, extraInfo);

}
