using ID.Application.Features.Images.Qry;
using ID.Application.Mediatr.Cqrslmps.Queries;
using ID.Domain.Utility.Exceptions;
using ID.GlobalSettings.Testing.Wrappers.DirectoryClass;
using ID.GlobalSettings.Testing.Wrappers.FileClass;
using ID.GlobalSettings.Testing.Wrappers.PathClass;
using Moq;
using Shouldly;

namespace ID.Application.Tests.Features.Images;

public abstract class AImgQryHandlerTestBase<THandler, TQuery>
    where THandler : AImgQryHandlerBase<TQuery>
    where TQuery : AIdQuery, new()
{
    protected Mock<IPathWrapper> pathMock = new();
    protected Mock<IDirectoryWrapper> directoryMock = new();
    protected Mock<IFileWrapper> fileMock = new();
    protected THandler handler;

    protected readonly string _imageName;
    protected readonly string _containingDirectory;
    protected readonly TQuery _request;

    protected AImgQryHandlerTestBase(Func<TQuery> queryBuilder, string imageName, string containingDirectory)
    {
        handler = (THandler)Activator.CreateInstance(typeof(THandler), pathMock.Object, directoryMock.Object, fileMock.Object)!;
        _imageName = imageName;
        _containingDirectory = containingDirectory;
        _request = queryBuilder();
    }

    protected TQuery CreateQuery() => new();

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenImageExists()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var buildDir = "test_build_dir";
        var imagePath = Path.Combine(buildDir, "Features", "Images", "Qry", _containingDirectory, _imageName);

        pathMock.Setup(p => p.GetDirectoryName(It.IsAny<string>())).Returns(buildDir);
        pathMock.Setup(p => p.Combine(It.IsAny<string[]>())).Returns(imagePath);
        directoryMock.Setup(p => p.Exists(It.IsAny<string>())).Returns(true);
        fileMock.Setup(p => p.Exists(It.IsAny<string>())).Returns(true);

        // Act
        var result = await handler.Handle(_request, cancellationToken);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Info.ShouldBe(imagePath);

    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenBuildDirNameDoesNotExist()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        pathMock.Setup(p => p.GetDirectoryName(It.IsAny<string>())).Returns((string?)null);

        // Act
        var result = await handler.Handle(_request, cancellationToken);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Exception.ShouldBeOfType<MyIdDirectoryNotFoundException>();
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenBuildDirDoesNotExist()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        pathMock.Setup(p => p.GetDirectoryName(It.IsAny<string>())).Returns("BasePath");
        pathMock.Setup(p => p.Exists(It.IsAny<string>())).Returns(false);

        // Act
        var result = await handler.Handle(_request, cancellationToken);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Exception.ShouldBeOfType<MyIdDirectoryNotFoundException>();
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenImageDoesNotExist()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var buildDir = "test_build_dir";
        var imagePath = Path.Combine(buildDir, "Features", "Images", "Qry", _containingDirectory, _imageName);


        pathMock.Setup(p => p.GetDirectoryName(It.IsAny<string>())).Returns(buildDir);
        pathMock.Setup(p => p.Combine(It.IsAny<string[]>())).Returns(imagePath);
        directoryMock.Setup(p => p.Exists(It.IsAny<string>())).Returns(true);
        fileMock.Setup(p => p.Exists(It.IsAny<string>())).Returns(false);

        // Act
        var result = await handler.Handle(_request, cancellationToken);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Exception.ShouldBeOfType<MyIdFileNotFoundException>();
    }

    //------------------------------------//

}
