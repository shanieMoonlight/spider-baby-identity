using ID.Application.Customers.Abstractions;
using ID.Application.Customers.Dtos.User;
using ID.Application.Customers.Features.Account.Cmd.RegCustomer;
using ID.Application.Customers.Features.Common.Dtos.User;
using ID.Domain.Entities.AppUsers;
using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Shouldly;

namespace ID.Application.Customers.Tests.Features.Account.Cmd.RegCustomer;

public class RegisterCustomerCmdHandlerTests
{
    private readonly Mock<IIdCustomerRegistrationService> _mockRegService;
    private readonly RegCustomerCmdHandler _handler;

    public RegisterCustomerCmdHandlerTests()
    {
        _mockRegService = new Mock<IIdCustomerRegistrationService>();
        _handler = new RegCustomerCmdHandler(_mockRegService.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnConvertedResult_WhenRegistrationIsSuccessful()
    {
        // Arrange
        var request = new RegisterCustomerCmd(new RegisterCustomerDto());
        var cancellationToken = CancellationToken.None;
        var createdUser = AppUserDataFactory.Create(Guid.NewGuid());
        var createdUserDto = createdUser.ToCustomerDto();
        var genResult = GenResult<AppUser>.Success(createdUser);

        _mockRegService.Setup(x => x.RegisterAsync(request.Dto, cancellationToken))
            .ReturnsAsync(genResult);

        // Act
        var result = await _handler.Handle(request, cancellationToken);

        // Assert
        result.Value.ShouldBeEquivalentTo(createdUserDto);
        _mockRegService.Verify(x => x.RegisterAsync(request.Dto, cancellationToken), Times.Once);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnFailureResult_WhenRegistrationFails()
    {
        // Arrange
        var request = new RegisterCustomerCmd(new RegisterCustomerDto());
        var expectedResult = GenResult<AppUser>.Failure("Registration failed");

        _mockRegService
            .Setup(x => x.RegisterAsync(It.IsAny<RegisterCustomerDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldBe("Registration failed");
    }

    //------------------------------------//

}//Cls
