using ID.Application.Features.Images.Qry;
using ID.GlobalSettings.Testing.Wrappers.DirectoryClass;
using ID.GlobalSettings.Testing.Wrappers.FileClass;
using ID.GlobalSettings.Testing.Wrappers.PathClass;
using MyResults;

namespace ID.Application.Features.Images.Qry.Welcome;

public class GetWelcomeImgQryHandler(IPathWrapper path, IDirectoryWrapper directory, IFileWrapper file)
    : AImgQryHandlerBase<GetWelcomeImgQry>(path, directory, file)
{
    public override Task<BasicResult> Handle(GetWelcomeImgQry request, CancellationToken cancellationToken)
    {
        var imageName = "welcome.jpg";
        var containingFolder = nameof(Welcome);
        var result = GetImage(containingFolder, imageName);
        return Task.FromResult(result);
    }
}

//C:\Users\Shaneyboy\source\repos\BeRepo\Apps\Tester\ID.Tester\Features\Images\Qry\Welcome\welcome.jpeg
//C:\Users\Shaneyboy\source\repos\BeRepo\Apps\Tester\ID.Tester\bin\Debug\net8.0\Features\Images\Qry\Welcome