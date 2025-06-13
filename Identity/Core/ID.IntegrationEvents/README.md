# ID.IntegrationEvents

 The `ID.IntegrationEvents` project is a library designed to handle integration events within the Identity system. 
 This is where all Id Integration events, abstractions and registration will live.
 It provides an interface for publishing events to an event bus, facilitating communication between different parts of the system.

 All IIdIntegrationEvent implementations must have a paramaterless ctor for Serialization.

## Requirements

- .NET 8.0


## Installation

To install the `ID.IntegrationEvents` library, add the library to to your project file dependencies:

Call service.AddIdEvents() in the ConfigureServices method of the Startup class in the Identity.Api project.

To Register all consumers in an assembly, so that they can be found by the publisher, you call
the method services.RegisterIdEventListeners([ ...list of assemblies with Listeners/Consumers]) in your startup/setup class.
This can be called multiple times to register multiple assemblies.


This also the place where the EventPubSub implementations are set.
Current options are Mediatr and MassTransit.

Abstract class AEventHandler<T> inherits from  MassTrasit.IConsumer<T> and , Mediatr.INotificationHandler<T>
so when you implement it the Listener can be triggered by both the MassTransit and Mediatr implementations.

Interface IIntegrationEvent extends Mediatr.INotification (inheritence is not required for MassTransit Events to work)

If you want to add a new Pub/Sub provider. Add a new implementation of IEventBus and Create a setup file that hooks up your new provider.
Update the EventSetup class to use your new provider.

The rest of the app is unchanged.



## Usage

### IEventBus Interface

The `IEventBus` interface defines a method for publishing events:

Just inject the IEventBus interface into your class and call the Publish method with the event you want to publish.
If you want to listen/consume an event, create a class that implements the AEventHandler<T> abstract class and register it's assemly using services.RegisterIdEventListeners([YourContainingAssembly])


## Project References

The project references the following projects:

- `ID.Domain`

## Contributing

Contributions are welcome! Please open an issue or submit a pull request.

## License

This project is licensed under the MIT License.

der the MIT License.