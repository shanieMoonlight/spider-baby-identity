This project contains the repository implementations for the domain layer of the MyId application.
This is a separate library from Id.Domain in order to keep the repositories internal to the MyId System.
Users of the MyId System should not have access to the repository implementations, only to the domain entities and interfaces defined in Id.Domain.
For users of the MyId System, we have ***Services for working with the domain layer.*** These services provide a higher-level API for interacting with the domain entities without exposing the underlying repository implementations.