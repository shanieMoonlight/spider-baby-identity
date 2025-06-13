using ID.Application.Features.Images.Qry.PhoneConfirmed;
using ID.Application.Features.Images.Qry.FallbackLogo;

namespace ID.Application.Tests.Features.Images;

public class GetPhoneConfirmedImgQryHandlerTests : AImgQryHandlerTestBase<GetPhoneConfirmedImgQryHandler, GetPhoneConfirmedImgQry>
{
    public GetPhoneConfirmedImgQryHandlerTests()
        : base(
            () => new GetPhoneConfirmedImgQry(),
            "phone_confirmed.jpg",
            "PhoneConfirmed"
        )
    { }

}//Cls