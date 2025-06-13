using ID.Application.Features.Images.Qry;
using ID.GlobalSettings.Testing.Wrappers.DirectoryClass;
using ID.GlobalSettings.Testing.Wrappers.FileClass;
using ID.GlobalSettings.Testing.Wrappers.PathClass;
using MyResults;

namespace ID.Application.Features.Images.Qry.PhoneConfirmed;

public class GetPhoneConfirmedImgQryHandler(IPathWrapper path, IDirectoryWrapper directory, IFileWrapper file)
    : AImgQryHandlerBase<GetPhoneConfirmedImgQry>(path, directory, file)
{
    public override Task<BasicResult> Handle(GetPhoneConfirmedImgQry request, CancellationToken cancellationToken)
    {
        var imageName = "phone_confirmed.jpg";
        var containingFolder = nameof(PhoneConfirmed);
        var result = GetImage(containingFolder, imageName);
        return Task.FromResult(result);
    }
}