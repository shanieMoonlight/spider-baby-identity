using FluentValidation;
using FluentValidation.Results;
using ID.Application.Mediatr.Behaviours.Validation;
using MediatR;
using Moq;
using MyResults;
using Shouldly;

namespace ID.Application.Tests.Mediatr.Pipeline.Validation;

public class TestBasicRequest : IRequest<BasicResult> { }

public class IdBasicValidationPipelineBehaviourTests
{
    private readonly Mock<IValidator<TestBasicRequest>> _validatorMock;
    private readonly IdValidationPipelineBehaviour<TestBasicRequest, BasicResult> _pipelineBehaviour;

    //------------------------------------//

    public IdBasicValidationPipelineBehaviourTests()
    {
        _validatorMock = new Mock<IValidator<TestBasicRequest>>();
        var validators = new List<IValidator<TestBasicRequest>> { _validatorMock.Object };
        _pipelineBehaviour = new IdValidationPipelineBehaviour<TestBasicRequest, BasicResult>(validators);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnNext_WhenNoValidationFailures()
    {
        // Arrange
        var request = new TestBasicRequest();
        var next = new Mock<RequestHandlerDelegate<BasicResult>>();
        next.Setup(n => n(It.IsAny<CancellationToken>())).ReturnsAsync(BasicResult.Success());

        _validatorMock.Setup(v => v.Validate(It.IsAny<ValidationContext<TestBasicRequest>>()))
                      .Returns(new ValidationResult());

        // Act
        var result = await _pipelineBehaviour.Handle(request, next.Object, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<BasicResult>();
        result.Succeeded.ShouldBeTrue();
        next.Verify(n => n(It.IsAny<CancellationToken>()), Times.Once);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnValidationResult_WhenValidationFailuresExist()
    {
        // Arrange
        var request = new TestBasicRequest();
        var next = new Mock<RequestHandlerDelegate<BasicResult>>();
        var failures = new List<ValidationFailure>
    {
        new("Property1", "Error1"),
        new("Property2", "Error2")
    };

        _validatorMock.Setup(v => v.Validate(It.IsAny<ValidationContext<TestBasicRequest>>()))
                      .Returns(new ValidationResult(failures));

        // Act
        var result = await _pipelineBehaviour.Handle(request, next.Object, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<BasicResult>();
        result.Succeeded.ShouldBeFalse();
        next.Verify(n => n(It.IsAny<CancellationToken>()), Times.Never);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturn_BadRequest_When_StateIsNotForbiddenOrUnathorize()
    {
        // Arrange
        var request = new TestBasicRequest();
        var next = new Mock<RequestHandlerDelegate<BasicResult>>();
        var failures = new List<ValidationFailure>
        {
            new("Property1", "Error1"){}
        };

        _validatorMock.Setup(v => v.Validate(It.IsAny<ValidationContext<TestBasicRequest>>()))
                      .Returns(new ValidationResult(failures));

        // Act
        var result = await _pipelineBehaviour.Handle(request, next.Object, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<BasicResult>();
        result.Succeeded.ShouldBeFalse();
        result.BadRequest.ShouldBeTrue();
        next.Verify(n => n(It.IsAny<CancellationToken>()), Times.Never);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturn_Forbidden_When_StateForbidden()
    {
        // Arrange
        var request = new TestBasicRequest();
        var next = new Mock<RequestHandlerDelegate<BasicResult>>();
        var failures = new List<ValidationFailure>
        {
            new("Property1", "Error1"){CustomState = ValidationError.Forbidden}
        };

        _validatorMock.Setup(v => v.Validate(It.IsAny<ValidationContext<TestBasicRequest>>()))
                      .Returns(new ValidationResult(failures));

        // Act
        var result = await _pipelineBehaviour.Handle(request, next.Object, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<BasicResult>();
        result.Succeeded.ShouldBeFalse();
        result.Forbidden.ShouldBeTrue();
        result.BadRequest.ShouldBeFalse();
        next.Verify(n => n(It.IsAny<CancellationToken>()), Times.Never);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturn_Unauthorized_When_StateUnauthorized()
    {
        // Arrange
        var request = new TestBasicRequest();
        var next = new Mock<RequestHandlerDelegate<BasicResult>>();
        var failures = new List<ValidationFailure>
        {
            new("Property1", "Error1"){CustomState = ValidationError.Unauthorized}
        };

        _validatorMock.Setup(v => v.Validate(It.IsAny<ValidationContext<TestBasicRequest>>()))
                      .Returns(new ValidationResult(failures));

        // Act
        var result = await _pipelineBehaviour.Handle(request, next.Object, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<BasicResult>();
        result.Succeeded.ShouldBeFalse();
        result.Unauthorized.ShouldBeTrue();
        result.BadRequest.ShouldBeFalse();
        next.Verify(n => n(It.IsAny<CancellationToken>()), Times.Never);
    }

    //------------------------------------//



}//Cls

