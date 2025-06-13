using ID.Application.Features.Images.Qry;
using ID.GlobalSettings.Testing.Wrappers.DirectoryClass;
using ID.GlobalSettings.Testing.Wrappers.FileClass;
using ID.GlobalSettings.Testing.Wrappers.PathClass;
using MyResults;

namespace ID.Application.Features.Images.Qry.FallbackLogo;

public class GetFallbackLogoQryHandler(IPathWrapper path, IDirectoryWrapper directory, IFileWrapper file)
    : AImgQryHandlerBase<GetFallbackLogoQry>(path, directory, file)
{
    public override Task<BasicResult> Handle(GetFallbackLogoQry request, CancellationToken cancellationToken)
    {
        var imageName = "fallback_logo.png";
        var containingFolder = nameof(FallbackLogo);
        var result = GetImage(containingFolder, imageName);
        return Task.FromResult(result);
    }
}