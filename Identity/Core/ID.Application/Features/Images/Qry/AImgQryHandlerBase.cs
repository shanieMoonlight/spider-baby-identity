using ID.Application.Mediatr.CqrsAbs;
using ID.Application.Mediatr.Cqrslmps.Queries;
using ID.GlobalSettings.Testing.Wrappers.DirectoryClass;
using ID.GlobalSettings.Testing.Wrappers.FileClass;
using ID.GlobalSettings.Testing.Wrappers.PathClass;
using ID.Domain.Utility.Exceptions;
using MyResults;
using System.Reflection;

namespace ID.Application.Features.Images.Qry;

public abstract class AImgQryHandlerBase<TImgQuery>(IPathWrapper _path, IDirectoryWrapper _directory, IFileWrapper _file)
    : IIdQueryHandler<TImgQuery> where TImgQuery : AIdQuery
{
    public abstract Task<BasicResult> Handle(TImgQuery request, CancellationToken cancellationToken);

    protected BasicResult GetImage(string containingFolder, string imageName)
    {
        var buildDir = _path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (buildDir == null || !_directory.Exists(buildDir))
            return BasicResult.Failure(new MyIdDirectoryNotFoundException(buildDir ?? imageName));

        var imagePath = _path.Combine(buildDir, "Features", "Images", "Qry", containingFolder, imageName);
        if (!_file.Exists(imagePath))
            return BasicResult.Failure(new MyIdFileNotFoundException(imageName));

        return BasicResult.Success(imagePath);
    }
}
