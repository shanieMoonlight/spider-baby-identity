
using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.GlobalSettings.Utility;
using StringHelpers;

namespace ID.Application.Features.Account.Cmd.Mfa.TwoFactorDisable;
public class TwoFactorDisableCmdValidator : IsAuthenticatedValidator<TwoFactorDisableCmd>
{ }

