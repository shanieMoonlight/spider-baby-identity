using ID.Application.AppAbs.FromApp;

namespace ID.Infrastructure.Services.FromApp;
internal class FromMobileApp : IIsFromMobileApp
{
    public bool IsFromApp { get; set; }
}
