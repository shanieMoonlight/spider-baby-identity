//using ID.Domain.Mediatr.Behaviours;
//using ID.Domain.Mediatr.Behaviours.Common;
//using ID.Domain.Mediatr.Requests;
//using ID.Domain.Setup;
//using ID.Tests.Data.Fixtures;
//using MediatR;
//using Moq;
//using MyResults;
//using Shouldly;

//namespace ID.Domain.Tests.Mediatr.Pipeline;



////=============================================================================//

//public record TestCustomerApplicationRequest : IIdCustomerApplicationRequest, IRequest<BasicResult>
//{ }


////=============================================================================//



//public class IdCustomerApplicationBehaviourTests(IdApplicationSettingsFixture _fixture)
//    : IClassFixture<IdApplicationSettingsFixture>
//{
//    [Fact]
//    public async Task Handle_ShouldReturnNotFoundResponse_WhenCustomerRegistrationIsNotAllowed()
//    {
//        // Arrange
//        _fixture.AllowCustomerRegistration = false;
//        var requestMock = new Mock<IIdCustomerApplicationRequest>();
//        var testRequest = new TestCustomerApplicationRequest { };

//        var nextMock = new Mock<RequestHandlerDelegate<BasicResult>>();
//        var behaviour = new IdCustomerApplicationBehaviour<TestCustomerApplicationRequest, BasicResult>();

//        // Act
//        var response = await behaviour.Handle(testRequest, nextMock.Object, default);

//        // Assert
//        response.ShouldBeOfType<BasicResult>();
//        response.ShouldBeEquivalentTo(ResponseProvider.GenerateNotFoundResponse<BasicResult>());
//        nextMock.Verify(n => n(), Times.Never);
//    }

//    //------------------------------------//

//    [Fact]
//    public async Task Handle_ShouldCallNext_WhenCustomerRegistrationIsAllowed()
//    {
//        // Arrange
//        _fixture.AllowCustomerRegistration = true;
//        var testRequest = new TestCustomerApplicationRequest { };
//        var nextMock = new Mock<RequestHandlerDelegate<BasicResult>>();
//        nextMock.Setup(n => n()).ReturnsAsync(BasicResult.Success());
//        var behaviour = new IdCustomerApplicationBehaviour<TestCustomerApplicationRequest, BasicResult>();

//        // Act
//        var response = await behaviour.Handle(testRequest, nextMock.Object, default);

//        // Assert
//        response.ShouldBeOfType<BasicResult>();
//        nextMock.Verify(n => n(), Times.Once);
//    }

//    //------------------------------------//

//}//Cls