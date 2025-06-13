//using System.Threading;
//using System.Threading.Tasks;
//using ID.Application.Abstractions.ApplicationServices;
//using ID.Application.Dtos.User;
//using ID.Application.Features.Account.Cmd.Customer.RegCustomer;
//using ID.Tests.Data.Factories;
//using Moq;
//using Shouldly;
//using Xunit;

//namespace ID.Application.Tests.Features.Account.Cmd.Customer.RegCustomer
//{
//    public class RegCustomerCmdHandlerTests
//    {
//        private readonly Mock<IIdCustomerRegistrationService> _mockRegService;
//        private readonly RegCustomerCmdHandler _handler;

//        public RegCustomerCmdHandlerTests()
//        {
//            _mockRegService = new Mock<IIdCustomerRegistrationService>();
//            _handler = new RegCustomerCmdHandler(_mockRegService.Object);
//        }

//        [Fact]
//        public async Task Handle_ShouldReturnSuccessResult_WhenRegistrationSucceeds()
//        {
//            // Arrange
//            var request = new RegCustomerCmd
//            {
//                Dto = new RegisterCustomerDto
//                {
//                    Email = "test@example.com",
//                    Username = "testuser",
//                    Phone = "1234567890",
//                    FirstName = "Test",
//                    LastName = "User",
//                    Password = "Password123!",
//                    ConfirmPassword = "Password123!",
//                    TeamPosition = 1
//                }
//            };

//            var appUser = AppUserDataFactory.Create(Guid.NewGuid());
//            var genResult = new GenResult<AppUser>(appUser);

//            _mockRegService.Setup(x => x.RegisterAsync(
//                It.IsAny<EmailAddress>(),
//                It.IsAny<UsernameNullable>(),
//                It.IsAny<PhoneNullable>(),
//                It.IsAny<FirstNameNullable>(),
//                It.IsAny<LastNameNullable>(),
//                It.IsAny<Password>(),
//                It.IsAny<ConfirmPassword>(),
//                It.IsAny<TeamPositionNullable>(),
//                It.IsAny<Guid?>(),
//                It.IsAny<CancellationToken>()
//            )).ReturnsAsync(genResult);

//            // Act
//            var result = await _handler.Handle(request, CancellationToken.None);

//            // Assert
//            result.Succeeded.ShouldBeTrue();
//            result.Value.ShouldNotBeNull();
//            result.Value.Email.ShouldBe(request.Dto.Email);
//        }

//        [Fact]
//        public async Task Handle_ShouldReturnFailureResult_WhenRegistrationFails()
//        {
//            // Arrange
//            var request = new RegCustomerCmd
//            {
//                Dto = new RegisterCustomerDto
//                {
//                    Email = "test@example.com",
//                    Username = "testuser",
//                    Phone = "1234567890",
//                    FirstName = "Test",
//                    LastName = "User",
//                    Password = "Password123!",
//                    ConfirmPassword = "Password123!",
//                    TeamPosition = 1
//                }
//            };

//            var genResult = GenResult<AppUser>.Failure("Registration failed");

//            _mockRegService.Setup(x => x.RegisterAsync(
//                It.IsAny<EmailAddress>(),
//                It.IsAny<UsernameNullable>(),
//                It.IsAny<PhoneNullable>(),
//                It.IsAny<FirstNameNullable>(),
//                It.IsAny<LastNameNullable>(),
//                It.IsAny<Password>(),
//                It.IsAny<ConfirmPassword>(),
//                It.IsAny<TeamPositionNullable>(),
//                It.IsAny<Guid?>(),
//                It.IsAny<CancellationToken>()
//            )).ReturnsAsync(genResult);

//            // Act
//            var result = await _handler.Handle(request, CancellationToken.None);

//            // Assert
//            result.Succeeded.ShouldBeFalse();
//            result.Info.ShouldBe("Registration failed");
//        }
//    }
//}
