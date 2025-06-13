using ID.Application.Mediatr.Behaviours.Exceptions;
using ID.Application.Tests.Mediatr.Pipeline.Exceptions.Helpers;
using ID.Domain.Utility.Exceptions;
using ID.Tests.Data.Factories;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using Moq;
using MyResults;
using Shouldly;
using static MyResults.BasicResult;

namespace ID.Application.Tests.Mediatr.Pipeline.Exceptions;


public class IdRequestExceptionHandlerTests
{

    private readonly Mock<ILogger<IdRequestExceptionHandler<TestRequest, BasicResult, MyIdException>>> _loggerMock;
    private readonly IdRequestExceptionHandler<TestRequest, BasicResult, MyIdException> _sut;



    public IdRequestExceptionHandlerTests()
    {
        _loggerMock = new Mock<ILogger<IdRequestExceptionHandler<TestRequest, BasicResult, MyIdException>>>();
        _sut = new IdRequestExceptionHandler<TestRequest, BasicResult, MyIdException>(_loggerMock.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_GivenInvalidTeamNameException_ShouldCreateBadRequestResponse()
    {
        // Arrange
        var request = new TestRequest();
        var exception = new InvalidTeamNameException("Invalid team name");
        var state = new RequestExceptionHandlerState<BasicResult>();
        var cancellationToken = CancellationToken.None;

        // Act
        await _sut.Handle(request, exception, state, cancellationToken);

        // Assert
        state.Response.ShouldNotBeNull();
        state.Response.Succeeded.ShouldBeFalse();
        state.Response.Status.ShouldBe(ResultStatus.BadRequest);
        state.Response.Info.ShouldNotBeNullOrWhiteSpace();

    }

    [Fact]
    public async Task Handle_GivenDeviceLimitExceededException_ShouldCreateBadRequestResponse()
    {
        // Arrange
        var request = new TestRequest();
        var sub = SubscriptionDataFactory.Create();
        var exception = new DeviceLimitExceededException(sub);
        var state = new RequestExceptionHandlerState<BasicResult>();
        var cancellationToken = CancellationToken.None;

        // Act
        await _sut.Handle(request, exception, state, cancellationToken);

        // Assert
        state.Response.ShouldNotBeNull();
        state.Response.Succeeded.ShouldBeFalse();
        state.Response.Status.ShouldBe(ResultStatus.BadRequest);
        //state.Response.Info.ShouldBe("Device limit exceeded");
        state.Handled.ShouldBeTrue();

    }



    [Fact]
    public async Task Handle_GivenMyIdFileNotFoundException_ShouldCreateFailureResponse()
    {
        // Arrange
        var request = new TestRequest();
        var exception = new MyIdFileNotFoundException("File not found");
        var state = new RequestExceptionHandlerState<BasicResult>();
        var cancellationToken = CancellationToken.None;

        // Act
        await _sut.Handle(request, exception, state, cancellationToken);

        // Assert
        state.Response.ShouldNotBeNull();
        state.Response.Succeeded.ShouldBeFalse();
        state.Response.Status.ShouldBe(ResultStatus.Failure);
        state.Response.Exception.ShouldBe(exception);
        state.Handled.ShouldBeTrue();
    }



    [Fact]
    public async Task Handle_GivenUnrecognizedException_ShouldCreateFailureResponse()
    {
        // Arrange
        var request = new TestRequest();
        var exception = new MyIdException("Generic exception");
        var state = new RequestExceptionHandlerState<BasicResult>();
        var cancellationToken = CancellationToken.None;

        // Act
        await _sut.Handle(request, exception, state, cancellationToken);

        // Assert
        state.Response.ShouldNotBeNull();
        state.Response.Succeeded.ShouldBeFalse();
        state.Response.Status.ShouldBe(ResultStatus.Failure);
        state.Handled.ShouldBeTrue();
        state.Response.Exception.ShouldBe(exception);

    }



    [Fact]
    public async Task Handle_WithGenericResult_ShouldHandleCorrectly()
    {
        // Arrange
        var genericLoggerMock = new Mock<ILogger<IdRequestExceptionHandler<TestRequest, GenResult<TestData>, MyIdException>>>();
        var handler = new IdRequestExceptionHandler<TestRequest, GenResult<TestData>, MyIdException>(genericLoggerMock.Object);
        var request = new TestRequest();
        var exception = new MyIdException("Generic exception");
        var state = new RequestExceptionHandlerState<GenResult<TestData>>();
        var cancellationToken = CancellationToken.None;

        // Act
        await handler.Handle(request, exception, state, cancellationToken);

        // Assert
        state.Response.ShouldNotBeNull();
        state.Response.Succeeded.ShouldBeFalse();
        state.Response.Status.ShouldBe(ResultStatus.Failure);
        state.Response.Exception.ShouldBe(exception);
        state.Response.Value.ShouldBe(default);
        state.Handled.ShouldBeTrue();
    }


    //------------------------------------//


    public static IEnumerable<object[]> ExceptionHandlers()
    {
        yield return new object[] { MyIdFileNotFoundExceptionTestHelper.Params, };
        yield return new object[] { MyIdDirectoryNotFoundExceptionTestHelper.Params, };
        yield return new object[] { DeviceLimitExceededExceptionTestHelper.Params, };
        yield return new object[] { PingExceptionTestHelper.Params, };
        yield return new object[] { InvalidTeamNameExceptionTestHelper.Params, };
    }


    [Theory]
    [MemberData(nameof(ExceptionHandlers))]
    public async Task Should_run_exception_handler(TestParamaters testParams)
    {
        var mediator = testParams.ContainerGenerator()
            .GetInstance<IMediator>();

        var response = await mediator.Send(new TestExceptionsRequest("Test Sé"));

        testParams.Challenger(response);
    }

    //------------------------------------//


    // Test classes
    public class TestRequest { }
    public class TestData { }

}//Cls