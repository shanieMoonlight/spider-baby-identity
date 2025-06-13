# ID.Application.Customers

This project is part of the Identity system and provides functionality for managing customer-related operations within the application. 
It includes various features, commands,controllers and setup extensions to handle customer registration, validation, and other related tasks.

## Project Structure

- **Target Framework**: .NET 8.0
- **Dependencies**:
  - `FluentValidation`
  - `FluentValidation.DependencyInjectionExtensions`
  - `GoogleAuthenticator`
  - `MassTransit`
  - `ClArch.ValueObjects`
  - `EnumHelpers`
  - `MediatrHelpers`
  - `TestingHelpers`
  - `LoggingHelpers`
  - `MyResults`
  - `ID.Application`
  - `ID.Domain`

## Features

### Setup Extensions

The project includes setup extensions to configure customer-related services in the application. 
These extensions can be used to add necessary services and configurations during the application startup.

### Commands

The project provides various commands to handle customer-related operations such as registration, delete, update, and more. 
These commands are implemented using the MediatR library to support CQRS (Command Query Responsibility Segregation) pattern.

### Validators

The project includes validators to ensure the correctness and validity of customer-related data. These validators are implemented using the FluentValidation library.

## Usage

To use the features provided by this project, you need to reference it in your application and configure the necessary services.

### Example

Here is an example of how to use the `ID.Application.Customers` library in your application:
        
        using ID.Application.Customers.Setup; using Microsoft.Extensions.DependencyInjection;

        Services.AddMyIdCustomers(config =>
           {
               config.MaxTeamPosition = 3; //Default = 1 => no customer teams
               config.MinTeamPosition = 1; //Default = 1 => no customer teams
               config.MaxTeamSize = 10;    //Default = null => no limit
           })



    
## Contributing

If you would like to contribute to this project, please follow these steps:

1. Fork the repository.
2. Create a new branch for your feature or bugfix.
3. Write your code and tests.
4. Submit a pull request with a description of your changes.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.

