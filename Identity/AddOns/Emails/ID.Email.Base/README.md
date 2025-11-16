# ID.Email.Base — Adding a new EmailSpec and EmailConsumer

This document explains how to add a new email type (spec + consumer) to the `ID.Email.Base` add-on. The codebase uses a single template generator entrypoint: `IEmailDetailsTemplateGenerator.GenerateFromSpecAsync(IEmailSpec)` and a small set of helpers to produce `IEmailDetails` instances.

Summary

- Create a new `IEmailSpec` under `LocalImps/Specs` that knows how to build an `IEmailDetails`.
- Add HTML templates under `Assets/html-templates/...` and reference them from the spec.  (If using a template)
- Implement a new event consumer that constructs the spec and calls `GenerateFromSpecAsync`.
- Write unit tests that mock `IEmailDetailsTemplateGenerator.GenerateFromSpecAsync(It.IsAny<IEmailSpec>())`.

Why this pattern?

Using `IEmailSpec` keeps template-building logic isolated and testable. The `IEmailDetailsTemplateGenerator` is the single point that translates specs into `IEmailDetails`. Consumers only need to create specs and call `GenerateFromSpecAsync`, keeping them small and focused on event handling.

Step-by-step

1. Add HTML templates
   - Place any HTML files in `Assets/html-templates/<feature>/...`.
   - Pick placeholder keys from `EmailPlaceholders` (common placeholders are already defined).

2. Create an EmailSpec
   - Location: `Identity\AddOns\Emails\ID.Email.Base\LocalImps\Specs`
   - Name: `FeatureNameSpec` (or `XxxEmailSpec`).
   - Implement `IEmailSpec` and its method:
     Task<IEmailDetails> BuildAsync(IdGlobalOptions globalOptions, ITemplateHelpers templateHelpers, IdEmailBaseOptions emailOptions)
   - Use `templateHelpers.ReadAndReplaceTemplateAsync(templatePath, placeholders)` to render content, or `GenerateTemplateWithCallback` for callback-link templates.

   Minimal example:

   ```csharp
   internal sealed class ExampleFeatureSpec(string toName, string toAddress, string someArg) : IEmailSpec
   {
       private const string _template_path = @"Assets\html-templates\Example\ExampleTemplate.html";

       public async Task<IEmailDetails> BuildAsync(IdGlobalOptions globalOptions, ITemplateHelpers templateHelpers, IdEmailBaseOptions emailOptions)
       {
           var message = await templateHelpers.ReadAndReplaceTemplateAsync(_template_path, new Dictionary<string, string>
           {
               { EmailPlaceholders.PLACEHOLDER_USERNAME, toName },
               { "PLACEHOLDER_EXTRA", someArg }
           });

           return new EmailDetails(
               EmailType.HTML,
               message,
               "Example Subject",
               toAddress,
               emailOptions.BccAddresses,
               emailOptions.FromAddress,
               emailOptions.FromName
           );
       }
   }
   ```

3. Create an EventListener / Consumer
   - Location: `Identity\AddOns\Emails\ID.Email.Base\EventListeners\<Feature>`
   - Inject: `IEmailDetailsTemplateGenerator`, `IIdEmailService`, `ILogger<T>`.
   - Construct the spec from the incoming integration event and call `GenerateFromSpecAsync(spec)`.
   - Send the returned `IEmailDetails` with `_emailService.SendEmailAsync(details)` and handle the `BasicResult`.

   Consumer snippet:

   ```csharp
   var spec = new ExampleFeatureSpec(ev.Name, ev.Email, ev.SomeArg);

   IEmailDetails details;
   try
   {
       details = await _templateGenerator.GenerateFromSpecAsync(spec);
   }
   catch(Exception ex)
   {
       ExceptionUtils.VerifyExceptionLogging(_logger, IdErrorEvents.Email.Example, ex);
       return;
   }

   var result = await _emailService.SendEmailAsync(details);
   if (!result.Succeeded)
   {
       ExceptionUtils.VerifyBasicResultLogging(_logger, IdErrorEvents.Email.Example, result);
   }
   ```

4. Testing
   - Unit tests should mock `IEmailDetailsTemplateGenerator.GenerateFromSpecAsync(It.IsAny<IEmailSpec>())` and return a mock `IEmailDetails` when generation succeeds.
   - For template generation failure tests, make the mocked `GenerateFromSpecAsync` throw an exception.
   - For email service failure tests, return a failing `BasicResult` from `IIdEmailService.SendEmailAsync` and verify logging using `ExceptionUtils` helpers already used in tests.

Testing example (arrange):

```csharp
_templateGeneratorMock.Setup(x => x.GenerateFromSpecAsync(It.IsAny<IEmailSpec>()))
    .ReturnsAsync(_emailDetailsMock.Object);

_emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
    .ReturnsAsync(BasicResult.Success());
```

Notes and gotchas

- Keep spec classes small and focused on building the email. Avoid putting business logic in them.
- The spec has access to global options and email options — use them for route building and "from" address data.
- Reuse `EmailPlaceholders` keys to keep templates consistent.

Where to look for examples

- Specs:
  - `LocalImps\Specs\TwoFactorSpec.cs`
  - `LocalImps\Specs\TwoFactorGoogleAuthSpec.cs`
  - `LocalImps\Specs\PasswordResetSpec.cs`
  - `LocalImps\Specs\SubscriptionPausedSpec.cs`

- Consumers:
  - `EventListeners\TwoFactor\TwoFactorEmailRequestConsumer.cs`
  - `EventListeners\EmailConfirmation\EmailConfirmationConsumer.cs`
  - `EventListeners\ForgotPwd\ForgotPwdConsumer.cs`
  - `EventListeners\Subscriptions\SubscriptionsPausedConsumer.cs`


