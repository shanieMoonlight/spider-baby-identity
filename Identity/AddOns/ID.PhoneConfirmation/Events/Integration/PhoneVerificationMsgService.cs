using MyResults;
//using ID.Domain.Utility;
//using ID.Domain.Entities.AppUsers;
//using ID.Domain.Entities.Teams;
//using ID.PhoneConfirmation.Setup;
//using StringHelpers;
//using ID.PhoneConfirmation.Utility;
//using ID.Application.AppAbs.Messaging;
//using ID.Domain.Utility.Messages;
//using ID.GlobalSettings.Setup.Settings;


//namespace ID.PhoneConfirmation.Events.Integration;
//internal class PhoneVerificationMsgService(IIdSmsService smsService) 
//{
//    public async Task<BasicResult> SendMsgAsync(AppUser user, TeamType teamType, string token)
//    {
//        if (user.PhoneNumber.IsNullOrWhiteSpace())
//            return BasicResult.Failure(IDMsgs.Error.Phone.USER_WITHOUT_PHONE(user));

//        var msg = teamType == TeamType.Customer
//            ? CustomerConfirmationMsg(user, token)
//            : MntcConfirmationMsg(user, token);

//        await smsService.SendMsgAsync(user.PhoneNumber!, msg);
//        return BasicResult.Success();
//    }

//    //------------------------------------//

//    private static string MntcConfirmationMsg(AppUser user, string token)
//    {
//        string phoneConfirmationAddress = UrlBuilder.Combine(IdGlobalSettings.Customers.CustomerAccountsUrl, IdPhoneConstants.PhoneRoutes.ConfirmPhone);
//        return $"{phoneConfirmationAddress}?{IdPhoneConstants.PhoneRoutes.Params.UserId}={user.Id}&{IdPhoneConstants.PhoneRoutes.Params.ConfirmationToken}={token}";
//    }

//    //- - - - - - - - - - - - - - - - - - //

//    private static string CustomerConfirmationMsg(AppUser user, string token)
//    {
//        string phoneConfirmationAddress = UrlBuilder.Combine(IdGlobalSettings.MntcAccountsUrl, IdPhoneConstants.PhoneRoutes.ConfirmPhone);
//        return $"{phoneConfirmationAddress}?{IdPhoneConstants.PhoneRoutes.Params.UserId}={user.Id}&{IdPhoneConstants.PhoneRoutes.Params.ConfirmationToken}={token}";
//    }

//    //------------------------------------//

//}//Cls