using FluentValidation;
using FluentValidation.Results;
using ID.Application.Mediatr.Behaviours.Validation;
using ID.Tests.Data.Factories;
using MediatR;
using Moq;
using MyResults;
using Shouldly;

namespace ID.Application.Tests.Mediatr.Pipeline.Validation;

public class TestGenRequest : IRequest<GenResult<int>> { }

public class IdGenValidationPipelineBehaviourTests
{
    private readonly Mock<IValidator<TestGenRequest>> _validatorMock;
    private readonly IdValidationPipelineBehaviour<TestGenRequest, GenResult<int>> _pipelineBehaviour;

    //------------------------------------//

    public IdGenValidationPipelineBehaviourTests()
    {
        _validatorMock = new Mock<IValidator<TestGenRequest>>();
        var validators = new List<IValidator<TestGenRequest>> { _validatorMock.Object };
        _pipelineBehaviour = new IdValidationPipelineBehaviour<TestGenRequest, GenResult<int>>(validators);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnNext_WhenNoValidationFailures()
    {
        // Arrange
        var request = new TestGenRequest();
        var next = new Mock<RequestHandlerDelegate<GenResult<int>>>();
        next.Setup(n => n(CancellationToken.None)).ReturnsAsync(GenResult<int>
            .Success(666));

        _validatorMock.Setup(v => v.Validate(It.IsAny<ValidationContext<TestGenRequest>>()))
                      .Returns(new ValidationResult());

        // Act
        var result = await _pipelineBehaviour.Handle(request, next.Object, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<GenResult<int>>();
        result.Succeeded.ShouldBeTrue();
        next.Verify(n => n(CancellationToken.None), Times.Once);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnValidationResult_WhenValidationFailuresExist()
    {
        // Arrange
        var request = new TestGenRequest();
        var next = new Mock<RequestHandlerDelegate<GenResult<int>>>();
        var failures = new List<ValidationFailure>
    {
        new("Property1", "Error1"),
        new("Property2", "Error2")
    };

        _validatorMock.Setup(v => v.Validate(It.IsAny<ValidationContext<TestGenRequest>>()))
                      .Returns(new ValidationResult(failures));

        // Act
        var result = await _pipelineBehaviour.Handle(request, next.Object, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<GenResult<int>>();
        result.Succeeded.ShouldBeFalse();
        next.Verify(n => n(It.IsAny<CancellationToken>()), Times.Never);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturn_BadRequest_When_StateIsNotForbiddenOrUnathorize()
    {
        // Arrange
        var request = new TestGenRequest();
        var next = new Mock<RequestHandlerDelegate<GenResult<int>>>();
        var failures = new List<ValidationFailure>
        {
            new("Property1", "Error1"){}
        };

        _validatorMock.Setup(v => v.Validate(It.IsAny<ValidationContext<TestGenRequest>>()))
                      .Returns(new ValidationResult(failures));

        // Act
        var result = await _pipelineBehaviour.Handle(request, next.Object, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<GenResult<int>>();
        result.Succeeded.ShouldBeFalse();
        result.BadRequest.ShouldBeTrue();
        next.Verify(n => n(It.IsAny<CancellationToken>()), Times.Never);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturn_Forbidden_When_StateForbidden()
    {
        // Arrange
        var request = new TestGenRequest();
        var next = new Mock<RequestHandlerDelegate<GenResult<int>>>();
        var failures = new List<ValidationFailure>
        {
            new("Property1", "Error1"){CustomState = ValidationError.Forbidden}
        };

        _validatorMock.Setup(v => v.Validate(It.IsAny<ValidationContext<TestGenRequest>>()))
                      .Returns(new ValidationResult(failures));

        // Act
        var result = await _pipelineBehaviour.Handle(request, next.Object, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<GenResult<int>>();
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
        var request = new TestGenRequest();
        var next = new Mock<RequestHandlerDelegate<GenResult<int>>>();
        var failures = new List<ValidationFailure>
        {
            new("Property1", "Error1"){CustomState = ValidationError.Unauthorized}
        };

        _validatorMock.Setup(v => v.Validate(It.IsAny<ValidationContext<TestGenRequest>>()))
                      .Returns(new ValidationResult(failures));

        // Act
        var result = await _pipelineBehaviour.Handle(request, next.Object, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<GenResult<int>>();
        result.Succeeded.ShouldBeFalse();
        result.Unauthorized.ShouldBeTrue();
        result.BadRequest.ShouldBeFalse();
        next.Verify(n => n(It.IsAny<CancellationToken>()), Times.Never);
    }

    //------------------------------------//



}//Cls

