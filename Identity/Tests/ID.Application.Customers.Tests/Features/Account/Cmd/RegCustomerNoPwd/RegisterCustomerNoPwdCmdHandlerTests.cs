using ID.Application.Customers.Abstractions;
using ID.Application.Customers.Dtos.User;
using ID.Application.Customers.Features.Account.Cmd.RegCustomerNoPwd;
using ID.Application.Customers.Features.Common.Dtos.User;
using ID.Domain.Entities.AppUsers;
using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Shouldly;

namespace ID.Application.Customers.Tests.Features.Account.Cmd.RegCustomerNoPwd;

public class RegisterCustomerNoPwdCmdHandlerTests
{
    private readonly Mock<IIdCustomerRegistrationService> _mockRegService;
    private readonly RegisterCustomerNoPwdCmdHandler _handler;

    public RegisterCustomerNoPwdCmdHandlerTests()
    {
        _mockRegService = new Mock<IIdCustomerRegistrationService>();
        _handler = new RegisterCustomerNoPwdCmdHandler(_mockRegService.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnSuccessResult_WhenRegistrationIsSuccessful()
    {
        // Arrange
        var request = new RegisterCustomerNoPwdCmd(new RegisterCustomer_NoPwdDto());
        var expectedUser = AppUserDataFactory.Create(Guid.NewGuid());
        var expectedResult = GenResult<AppUser>.Success(expectedUser);

        _mockRegService
            .Setup(x => x.Register_NoPwd_Async(It.IsAny<RegisterCustomer_NoPwdDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldBeEquivalentTo(expectedUser.ToCustomerDto());
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnFailureResult_WhenRegistrationFails()
    {
        // Arrange
        var request = new RegisterCustomerNoPwdCmd(new RegisterCustomer_NoPwdDto());
        var expectedResult = GenResult<AppUser>.Failure("Registration failed");

        _mockRegService
            .Setup(x => x.Register_NoPwd_Async(It.IsAny<RegisterCustomer_NoPwdDto>(), It.IsAny<CancellationToken>()))
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
