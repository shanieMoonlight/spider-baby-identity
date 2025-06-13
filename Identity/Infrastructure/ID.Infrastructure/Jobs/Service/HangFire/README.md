MyId Hangfire Jobs System
# MyId Hangfire Jobs System

## Overview

The MyId Hangfire Jobs System provides an isolated background job processing infrastructure for the Identity Library. 
It maintains complete separation from any host application's job infrastructure through dedicated storage, queues, and processing servers.

## Architecture

### Key Features

- **Isolation** - Dedicated SQL schema, separate server, and isolated queues
- **Multiple Queue Support** - Priority-based job execution (Default, Priority queues)
- **Clean Abstractions** - Interface-based design enabling comprehensive testing
- **Role-Based Dashboard** - Security levels based on team membership

## Setup

### Service Registration
### Middleware Configuration
## Core Components

### 1. Hangfire Instance Provider

`IHangfireInstanceProvider` serves as the central access point to all Hangfire infrastructure:

Example usage:
```csharp
internal class HfBackgroundJobMgr(IHangfireInstanceProvider instanceProvider, string queue) : IHfBackgroundJobMgr
{
    //Background job client that is connected to the MyId Hangfire storage. (Isolated)
    private readonly IBackgroundJobClientWrapper _backgroundJobClient = instanceProvider.BackgroundJobClient; 
    private readonly string _queue = queue;
    ...

}
```

### 2. Background Job Managers

For one-time jobs (immediate or delayed):
- **HfDefaultBackgroundJobMgr** - Standard priority job processing
- **HfPriorityBackgroundJobMgr** - High priority job processing


### 3. Recurring Job Managers

For scheduled jobs that run on a CRON schedule:
- **HfDefaultRecurringJobMgr** - Standard priority recurring jobs
- **HfPriorityRecurringJobMgr** - High priority recurring jobs
- 

## Security

Dashboard access is controlled via team-based authorization:
- **TeamType.Customer** - Anyone can access. (Probably not a good idea)
- **TeamType.Maintenance** - Maintenance team access (Maintenance teamm or higher)
- **TeamType.Super** - Full administrative access (Only super team)

## Implementation Details

### Storage Configuration
- Customized SQL Server storage with dedicated schema.  Isolated from any host application.
- Accessed through `IHangfireInstanceProvider` to ensure all Hangfire components use the same storage configuration.

### Job Processing
- Configurable timeout and invisibility settings
- Custom server name for job workers

### Dashboard Access
The Hangfire dashboard is available at the configured path (default: `/hangfire-myid`) and provides:
- Job monitoring
- Manual job triggering
- Performance metrics
- Error tracking

## Testing
The system is designed for comprehensive unit testing:
- Interface-based abstractions
- Mockable dependencies
- Test-specific job handlers

For detailed examples, see the unit tests in `ID.Infrastructure.Tests.Jobs.Service.HF` namespace.

---

**Note:** This system is intended for internal use by the MyId library only.
It is all designed to create an implementation for  the `IMyIdJobService` interface, which provides a clean abstraction for job management. 
Application code should interact with the `IMyIdJobService` interface rather than directly with the Hangfire components.
