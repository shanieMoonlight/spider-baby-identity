# Team Validator Token Pattern

## Overview

This folder contains the implementation of the **Validator Token Pattern** for Team domain operations. This pattern enforces strict validation of business rules before any database operations can be performed, providing compile-time safety and preventing invalid state changes.

## Purpose

The validator token pattern serves several critical purposes:

1. **Compile-time Safety**: Methods that modify Team state require a validation token, making it impossible to accidentally bypass business rule validation
2. **Explicit Validation**: All business rules must be explicitly validated before operations can proceed
3. **Immutable Proof**: Validation tokens are immutable proof that all business rules have been satisfied
4. **Centralized Logic**: All validation logic for a specific operation is contained in one place
5. **Testability**: Validation logic is isolated and easily testable

## How It Works

### 1. Validation Token Interface

All validation tokens implement `IValidationToken`:

```csharp
public interface IValidationToken
{
    Team Team { get; }
}
```

### 2. Validator Classes

Each domain operation has a corresponding validator class with:
- A nested `Token` class that implements `IValidationToken`
- A static `Validate` method that returns `GenResult<Token>`

Example structure:
```csharp
public partial class TeamValidators
{
    public sealed class MemberAddition
    {
        public sealed class Token : IValidationToken
        {
            // Token properties and constructor
        }

        public static GenResult<Token> Validate(Team team, AppUser member)
        {
            // Business rule validation logic
            // Returns success with token or failure with error message
        }
    }
}
```

### 3. Domain Method Integration

Team domain methods require a validation token to execute:

```csharp
/// <summary>
/// Adds a member to the team. Requires validation using TeamValidators.MemberAddition.Validate().
/// </summary>
/// <param name="token">Validation token proving all business rules have been satisfied</param>
/// <returns>Result indicating success or failure of the operation</returns>
public GenResult AddMember(TeamValidators.MemberAddition.Token token)
{
    // Implementation using validated data from token
}
```

## Current Validators

This system includes validators for the following Team operations:

- **MemberAddition**: Validates adding new members to a team
- **MemberRemoval**: Validates removing members from a team
- **MemberPositionUpdate**: Validates updating member positions
- **LeaderUpdate**: Validates leader changes
- **SubscriptionAddition**: Validates adding subscriptions to a team
- **SubscriptionRemoval**: Validates removing subscriptions from a team
- **PositionRangeUpdate**: Validates position range modifications

## Usage Example

```csharp
// Step 1: Validate the operation
var validationResult = TeamValidators.MemberAddition.Validate(team, newMember);
if (!validationResult.IsSuccess)
{
    // Handle validation failure
    return validationResult.Error;
}

// Step 2: Execute the operation with the validation token
var operationResult = team.AddMember(validationResult.Value);
if (!operationResult.IsSuccess)
{
    // Handle operation failure
    return operationResult.Error;
}

// Success - member has been added
```

## Benefits of This Approach

### 1. Prevents Invalid Operations
It's impossible to call domain methods without first validating business rules, eliminating a entire class of bugs.

### 2. Clear Error Messages
Validation failures return specific, actionable error messages that can be displayed to users or logged for debugging.

### 3. Separation of Concerns
- **Validators**: Focus solely on business rule validation
- **Domain Methods**: Focus solely on state mutation after validation
- **Application Layer**: Orchestrates validation and execution

### 4. Testability
Each validator can be tested independently with various scenarios:
- Valid inputs that should succeed
- Invalid inputs that should fail with specific error messages
- Edge cases and boundary conditions

### 5. Maintainability
When business rules change, the impact is localized to the relevant validator class. The domain method signatures remain stable.

### 6. Documentation
The pattern makes business rules explicit and self-documenting. Each validator clearly shows what conditions must be met for an operation to proceed.

## Design Principles

### Fail Fast
Validation happens before any state changes, catching problems early in the request pipeline.

### Immutable Tokens
Validation tokens are immutable and cannot be modified after creation, ensuring the validation cannot be tampered with.

### Single Responsibility
Each validator is responsible for one specific operation and its associated business rules.

### Explicit Dependencies
Domain methods explicitly declare their validation requirements through their method signatures.

## Error Handling

All validators return `GenResult<Token>` which provides:
- **Success State**: Contains the validation token needed for the operation
- **Failure State**: Contains detailed error information explaining why validation failed

This pattern integrates seamlessly with the application's error handling infrastructure and provides consistent, actionable feedback to users.

## Future Considerations

As the domain evolves, new validators can be added following the same pattern:

1. Create a new validator class in the `TeamValidators` partial class
2. Implement the `Token` nested class with required data
3. Implement the `Validate` static method with business rules
4. Update the corresponding domain method to require the validation token

This pattern scales naturally and maintains consistency across all domain operations.
