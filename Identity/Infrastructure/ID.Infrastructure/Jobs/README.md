# Job Implementation in ID.Infrastructure

This directory contains the implementation of all jobs defined in the Application layer. Each set of jobs has its own setup file in the containing folder, which hooks up the Dependency Injection (DI) and other necessary configurations.

## Overview

- **Job Implementations**: All job implementations defined in the Application layer are implemented here.
- **Setup Files**: Each set of jobs has its own setup file in the containing folder. These setup files are responsible for configuring Dependency Injection and other necessary setups.
- **IMyIdJobService**: The `IMyIdJobService` interface is implemented in this directory.
- **JobsSetup**: The `JobsSetup` class calls all the individual setup classes to get all the DI ready.

## Structure

- **Job Implementations**: The actual job classes that perform the tasks.
- **Setup Files**: Classes that configure the DI for the jobs.
- **IMyIdJobService**: Interface implementation for job services.
- **JobsSetup**: Aggregates all setup classes to ensure DI is configured correctly.

## Dependency Injection

Each job set has a corresponding setup file that configures the DI. This ensures that all dependencies are correctly injected and managed.

## Example

Here is an example of how a job and its setup might be structured:
# Job Implementation in ID.Infrastructure

This directory contains the implementation of all jobs defined in the Application layer. Each set of jobs has its own setup file in the containing folder, which hooks up the Dependency Injection (DI) and other necessary configurations.

## Overview

- **Job Implementations**: All job implementations defined in the Application layer are implemented here.
- **Setup Files**: Each set of jobs has its own setup file in the containing folder. These setup files are responsible for configuring Dependency Injection and other necessary setups.
- **IMyIdJobService**: The `IMyIdJobService` interface is implemented in this directory.
- **JobsSetup**: The `JobsSetup` class calls all the individual setup classes to get all the DI ready.

## Structure

- **Job Implementations**: The actual job classes that perform the tasks.
- **Setup Files**: Classes that configure the DI for the jobs.
- **IMyIdJobService**: Interface implementation for job services.
- **JobsSetup**: Aggregates all setup classes to ensure DI is configured correctly.

## Dependency Injection

Each job set has a corresponding setup file that configures the DI. This ensures that all dependencies are correctly injected and managed.

## Example

Here is an example of how a job and its setup might be structured:

### Job Implementation

This directory is essential for managing and executing background jobs within the application. The setup files ensure that all dependencies are correctly injected, making the job implementations robust and maintainable.

### Job Implementation
    internal class ProcessOldOutboxMsgs(IServiceProvider _serviceProvider, ILogger<ProcessOldOutboxMsgs> logger) : AProcess_Old_MyIdOutboxMsgs
    {

        [DisableConcurrentExecution(timeoutInSeconds: 300)]
        [DisplayName("MyId - Remove Old Outbox Msgs")]
        public override async Task HandleAsync()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var uow = scope.ServiceProvider.GetRequiredService<IIdUnitOfWork>();
                IIdentityOutboxMessageRepo _repo = uow.OutboxMessageRepo;
                await _repo.RemoveRangeAsync(new OutboxMsgsRemoveOlderThanSpec());

                await uow.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogException(e, MyIdLoggingEvents.JOBS.OLD_OUTBOX_MSGS_PROCESSING);
            }

        }
    

    }//Cls

### Inner Setup File
    internal static class Setup
    {

        public static IServiceCollection AddProcessOutboxMsgJobs(this IServiceCollection services)
        {
            return services
                .AddSingleton<AProcessMyIdOutboxMsgJob, ProcessOutboxMsgJob>()
                .AddSingleton<AProcess_Old_MyIdOutboxMsgs, ProcessOldOutboxMsgs>();
        }

    }//Cls


### Main Setup File (Make sure inner setup is reference by main setup)
    internal static class JobsSetup
    {
        public static IServiceCollection AddJobs(this IServiceCollection services)
        {
            return services
                .AddProcessOutboxMsgJobs(); <----------------- Add the setup file here
        }
    }


## Conclusion

This directory is essential for managing and executing background jobs within the application. 
The setup files ensure that all dependencies are correctly injected, making the job implementations robust and maintainable.