using ID.Application.Features.Images.Qry.Welcome;
using ID.Application.Features.Images.Qry.PhoneConfirmed;

namespace ID.Application.Tests.Features.Images;

public class GetWelcomeImgQryTests : AImgQryHandlerTestBase<GetWelcomeImgQryHandler, GetWelcomeImgQry>
{
    public GetWelcomeImgQryTests()
        : base(
            () => new GetWelcomeImgQry(),
            "welcome.jpg",
            "EmailConfirmed"
        )
    { }

}//Cls