Project-specific instructions for GitHub Copilot (repository-level)

Purpose
- Help Copilot produce suggestions and generate code tailored to this repository: a .NET 8, C# 12 identity solution with add-ons (OAuth providers, emails, phone confirmation), MediatR-based flows, and a large unit test suite.

High-level goals
- Preserve architecture and conventions already present (DI, domain-driven intent, small focused services, MediatR handlers).
- Prefer minimal, well-tested changes. New code should include unit tests where appropriate.

Tech stack & conventions
- Target framework: .NET 8.0. Language: C# 12. Keep nullable references enabled.
- Use dependency injection (Microsoft.Extensions.DependencyInjection) and register services via extension methods in the add-on or feature's Setup/DI folder.
- Http clients should use typed HttpClient registrations and resilience via the existing utilities (`AddMyIdOauthStandardResilienceHandler`/`ID.OAuth.Utils` patterns).
- Use Options pattern for configuration (`IOptions<T>` / `Options.Create` in tests). `Amazon` add-on already uses `IdOAuthAmazonOptions`.
- Use existing shared libs for value objects (`ClArch.ValueObjects`), MyResults (`GenResult<T>`), logging helpers, and serializers (`ID.OAuth.Utils.Serialization.UnixEpochSecondsJsonConverter`).
- Logging helpers: use `LoggingHelpers.LoggingExtensions` for consistent exception and result logging (`LogException`, `LogGenResultFailure`, `LogBasicResultFailure`, etc.). Prefer these helpers instead of ad-hoc log formatting.

Stylistic & naming conventions
- Keep filenames and namespaces aligned with folder layout (e.g., `ID.OAuth.Amazon.Services.Imps` for implementations).
- Public interfaces use `I{Name}`. Implementation classes use clear names and may be internal where appropriate (use `InternalsVisibleTo` for tests).
- Tests: xUnit + Moq + Shouldly. Test projects mirror folder structure under `Identity\\Tests`.
- Keep methods small and explicit; favor returning `GenResult<T>` for operations that require rich status/results.

Language preferences
- Prefer modern C# 12 features where they improve clarity and conciseness.
  - Use primary constructors for classes where it makes the intent clearer and reduces boilerplate. See C# primary constructors guidance: https://learn.microsoft.com/dotnet/csharp/whats-new/tutorials/primary-constructors
  - Use collection expressions when creating collections from other sequences or literals for concise initialization. See collection expressions: https://learn.microsoft.com/dotnet/csharp/language-reference/operators/collection-expressions
- Do not obscure readability with advanced syntax; prefer readability and consistency with surrounding code.

Testing guidance
- Add unit tests for any newly created logic. Use Moq to mock external dependencies and Shouldly for assertions.
- Tests must use `Moq` for mocking and `Shouldly` for assertions unless there's a strong reason to deviate.
- Test method formatting: separate individual test methods with a single comment line exactly: `//--------------------//` to improve readability across the test suite.
- Reuse `ID.Tests.Data.Factories` and `AppUserDataFactory` for domain test data.
- Use `CreateJsonOptions()` helpers or the shared `UnixEpochSecondsJsonConverter` for serialization tests.
- Test helper library: `Libs/TestingHelpers/TestingHelpers` is available and may be used for common test utilities, data generation, and helpers across test projects. Prefer using it when appropriate.

When editing existing code
- Read the file first and follow existing patterns; aim for smallest, least invasive change that fixes the problem.
- Run build locally and ensure tests compile. Keep public API and behavior stable unless changing intentionally.

Security & secrets
- Never add secrets, credentials, or real OAuth client secrets to the repo. Use configuration placeholders.

Commit & PR hints
- Keep commits small and focused. Tests must pass locally. Describe motivation and key changes in PR description.

If unsure
- Prefer creating a small reproducible test demonstrating the problem and propose a minimal change that satisfies the test.

Thank you — keep suggestions consistent with these conventions.