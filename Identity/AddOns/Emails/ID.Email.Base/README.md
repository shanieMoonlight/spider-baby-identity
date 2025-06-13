# ID.Email.Base

## Overview
The `ID.Email.Base` project contains all the event listeners and the `IIdEmailService` interface. This project is designed to handle email-related events and provide a standard interface for email services.

## Usage
To use the `ID.Email.Base` project in your library, follow these steps:

1. **Import the Project**:
   Add a reference to the `ID.Email.Base` project in your library.

2. **Implement the Interface**:
   Implement the `IIdEmailService` interface with your preferred email provider.

3. **Setup**:
   Call the setup method to initialize the email services and event listeners.
   Pass it to the Base Setup method: `services.AddIdEmailBase<MyEmailService>();`

## Event Listeners
The `ID.Email.Base` project includes various event listeners to handle email-related events. These listeners are automatically registered when you set up the project.

## Conclusion
By following the steps above, you can easily integrate the `ID.Email.Base` project into your library and use it with your preferred email provider.
