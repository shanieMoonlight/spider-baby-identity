# ClArch.ValueObjects

The `ClArch.ValueObjects` project is a library that provides some common value objects for the ClArch architecture. 
This library is built using .NET 8 and C# 12.0, and it includes several value objects that can be used to represent 
domain concepts in a type-safe manner.

ValueObjects will throw InvalidPropertyException if the value is invalid.

There are helper classes for creating value objects, such as `ValueObject<T>`, `StrValueObject`, etc., which provides a base class for creating

## Project Structure

The project structure is as follows:

- `ClArch.ValueObjects.csproj`: The project file that defines the dependencies and target framework for the project.

## Dependencies

The project has the following dependencies:

- `Microsoft.Extensions.DependencyInjection.Abstractions`: Provides abstractions for dependency injection.
- `StringHelpers`: A project reference to a helper library for string operations.
- `ValidationHelpers`: A project reference to a helper library for validation operations.

## Value Objects

### EmailVerifiedNullable

The `EmailVerifiedNullable` class is a value object that represents the verification status of an email, which can be `true`, `false`, or `null`. It inherits from the `ValueObject<bool?>` class.

#### Usage

Just import the library and use the predified ValueObjects or create a custom one using the Helper classes.

Example: 

    public class NotesContent : StrValueObject
    {
        public const int MaxLength = 3000;

        private NotesContent(string value) : base(value) { }

        public static NotesContent Create(string value)
        {
            Ensure.NotNullOrWhiteSpace(value, nameof(NotesContent));
            Ensure.MaxLength(value, MaxLength, nameof(NotesContent));

            return new(value);
        }

    }//Cls

    //====================================//

    public class NotesContentNullable : NullableStrValueObject
    {
        public const int MaxLength = 3000;

        private NotesContentNullable(string? value) : base(value) { }

        public static NotesContentNullable Create(string? value)
        {
            Ensure.MaxLength(value, MaxLength, nameof(NotesContent));

            return new(value);
        }

    }//Cls


If you need to change the settings for validation call services.AddValueObjects extension method with customized ValueObjectsSetupOptions.

    
    public void ConfigureServices(IServiceCollection services)
    {
         services.AddValueObjects(config =>
        {
            config.MaxLengthLastName = 50;
            config.MaxLengthFirstName = 50;
        });
    }
    
Use in Factory methods:
   
    public class MyCustomer : AuditableEntity
    {
        private MyCustomer(EmailAddress email)
        {
            Id = NewId.NextSequentialGuid();
            Email = email.Value.Trim();
            RaiseDomainEvent(new CustomerCreatedDomainEvent(this));
        }

        public static MyCustomer Create(
            EmailAddress email,
            PhoneNullableSafe phone,
            FirstName firstName,
            LastName lastName,
            CustomerLanguage language,
            CustomerLanguageIso languageIso,
            MediumNotesNullable? notes = null)
        {
            var customer = new MyCustomer(email)
                {
                    Phone = string.IsNullOrWhiteSpace(phone.Value) ? null : phone.Value,
                    FirstName = firstName.Value,
                    LastName = lastName.Value,
                    Notes = notes?.Value,
                    Language = language.Value,
                    LanguageIso = languageIso.Value,
                };

            return customer;
        }

    }//Cls
