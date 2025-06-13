using ID.Application.AppAbs.ApplicationServices.TwoFactor;
using ID.Application.AppAbs.Messaging;
using ID.Application.AppAbs.TokenVerificationServices;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Domain.Models;
using ID.Domain.Utility.Messages;
using ID.GlobalSettings.Setup.Options;
using ID.IntegrationEvents.Abstractions;
using ID.IntegrationEvents.Events.Account.TwoFactor;
using Microsoft.Extensions.Options;
using MyResults;
using StringHelpers;

namespace ID.Application.AppImps.TwoFactor;

public class TwoFactorMsgService(
    IIdSmsService smsService,
    IEventBus bus,
    ITwoFactorVerificationService<AppUser> _2FactorService,
    IOptions<IdGlobalOptions> _globalOptionsProvider)
    : ITwoFactorMsgService
{

    private readonly IdGlobalOptions _globalOptions = _globalOptionsProvider.Value;

    //-----------------------------//

    public async Task<GenResult<MfaResultData>> SendOTPFor2FactorAuth(Team team, AppUser user, TwoFactorProvider? tfProvider = null)
    {
        try
        {
            TwoFactorProvider provider = user.TwoFactorProvider;
            if (tfProvider != null && tfProvider.HasValue)
                provider = tfProvider.Value;

            var firstProvider = await _2FactorService.GetFirstValidTwoFactorProviderAsync(user);
            if (firstProvider.IsNullOrWhiteSpace())
                return await FallbackToEmail(team, user, IDMsgs.Error.TwoFactor.INVALID_2_FACTOR_PROVIDER);

            var canReceiveTfResult = CanReceiveTfMsg(user, provider);
            if (!canReceiveTfResult.Succeeded)
                return await FallbackToEmail(team, user, canReceiveTfResult.Info);


            string token = await _2FactorService.GenerateTwoFactorTokenAsync(team, user, firstProvider!);

            GenResult<MfaResultData> tfMsgResult = provider switch
            {
                TwoFactorProvider.Sms
                   => await SendSmsAsync(user, token),
                //TwoFactorProvider.WhatsApp
                //   => await whatsAppService.SendMsgAsync(user.PhoneNumber, message),
                TwoFactorProvider.Email
                   => await SendEmailAsync(user, token),
                TwoFactorProvider.AuthenticatorApp
                   => GenResult<MfaResultData>.Success(MfaResultData.Create(TwoFactorProvider.AuthenticatorApp)), //Do nothing,
                _
                   => await SendEmailAsync(user, token)
            };


            if (!tfMsgResult.Succeeded)
                return await FallbackToEmail(team, user, tfMsgResult.Info);

            return tfMsgResult;

        }
        catch (Exception e)
        {
            return GenResult<MfaResultData>.Failure(e);
        }

    }

    //-----------------------------//

    private static GenResult<bool> CanReceiveTfMsg(AppUser user, TwoFactorProvider provider)
    {
        if (provider == TwoFactorProvider.Sms)//|| provider == TwoFactorProvider.WhatsApp)
        {
            if (user.PhoneNumber.IsNullOrWhiteSpace())
                return GenResult<bool>.Failure(IDMsgs.Error.TwoFactor.NO_PHONE_FOR_TWO_FACTOR(user.UserName ?? user.Email ?? "???"));
        }

        if (provider == TwoFactorProvider.Email)
        {
            if (user.Email.IsNullOrWhiteSpace()) //This should never be true
                return GenResult<bool>.Failure(IDMsgs.Error.TwoFactor.NO_EMAIL_FOR_TWO_FACTOR(user.UserName ?? user.Email ?? "???"));
        }

        return GenResult<bool>.Success(true);
    }

    //-----------------------------//    

    //private async Task<GenResult<TotpResultData>> SendWhatsAppAsync(AppUser user, string token)
    //{
    //    string message = $"{IdApplicationSettings.ApplicationName} authentication token: {token}";
    //    await bus.Publish(new TwoFactorEmailRequestEvent(user, token), default);
    //    return new(TotpResultData.Create(TwoFactorProvider.WhatsApp));
    //}


    //- - - - - - - - - - - - - - -//

    private async Task<GenResult<MfaResultData>> SendSmsAsync(AppUser user, string token)
    {
        string message = $"{_globalOptions.ApplicationName} authentication token: {token}";
        var smsResult = await smsService.SendMsgAsync(user.PhoneNumber ?? "--", message); //"--" should throw

        if (!smsResult.Succeeded)
            return smsResult.Convert<MfaResultData>();

        return GenResult<MfaResultData>.Success(MfaResultData.Create(TwoFactorProvider.Sms));
    }


    //- - - - - - - - - - - - - - -//

    private async Task<GenResult<MfaResultData>> SendEmailAsync(AppUser user, string token)
    {
        await bus.Publish(new TwoFactorEmailRequestIntegrationEvent(user, token), default);
        return GenResult<MfaResultData>.Success(MfaResultData.Create(TwoFactorProvider.Email));
    }

    //-----------------------------//

    private async Task<GenResult<MfaResultData>> FallbackToEmail(Team team, AppUser user, string failureReason)
    {
        string NL = Environment.NewLine;
        string token = await _2FactorService.GenerateTwoFactorTokenAsync(team, user, "Email");

        //Try Emailing
        var emailResult = await SendEmailAsync(user, token);
        if (!emailResult.Succeeded)
            return emailResult.Convert<MfaResultData>();

        var emailedResult = MfaResultData.Create(
            TwoFactorProvider.Email,
            $"{IDMsgs.Error.TwoFactor.TWO_FACTOR_EMAIL_FALLBACK(user.Email ?? "NO-EMAIL")}{NL}Reason:{NL}{failureReason}");

        return GenResult<MfaResultData>.Success(emailedResult);
    }


}//Cls
