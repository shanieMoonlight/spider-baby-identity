using ID.Application.Mediatr.Behaviours.Exceptions;
using ID.Application.Tests.Mediatr.Pipeline.Exceptions.Logger;
using ID.Domain.Utility.Exceptions;
using Lamar;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using MyResults;

namespace ID.Application.Tests.Mediatr.Pipeline.Exceptions;

public class MyContainerProvider //: IContainerProvider
{
    private static Container? _container;
    private static readonly object _lock = new();

    /// <summary>
    /// Sets up an IOC container for the given handler and exception types.
    /// </summary>
    /// <typeparam name="THandler">The Handler that should be triggerd on a TestExceptionsRequest</typeparam>
    /// <typeparam name="TException">The type of Exception that the Exception handler will be listening for</typeparam>
    /// <param name="refresh">Get a new container or reuse the old one. Reuse when checking the CustomLogger</param>
    /// <returns>IOC conntainer</returns>
    public static Container GetContainer<THandler, TException>(bool refresh)
        where THandler : class, IRequestHandler<TestExceptionsRequest, BasicResult>
        where TException : MyIdException
    {
        if (_container != null && !refresh)
            return _container;

        lock (_lock)
        {
            _container = new(cfg =>
                {
                    cfg.For<ILoggerFactory>().Use(LoggerFactory.Create(builder => builder.AddConsole()));
                    cfg.For<IRequestHandler<TestExceptionsRequest, BasicResult>>().Use<THandler>();
                    cfg.ForSingletonOf<ILogger<IdRequestExceptionHandler<TestExceptionsRequest, BasicResult, TException>>>().Use<CustomLogger<IdRequestExceptionHandler<TestExceptionsRequest, BasicResult, TException>>>();
                    cfg.For<IRequestExceptionHandler<TestExceptionsRequest, BasicResult, TException>>().Use<IdRequestExceptionHandler<TestExceptionsRequest, BasicResult, TException>>();
                    cfg.For(typeof(IPipelineBehavior<,>)).Add(typeof(RequestExceptionProcessorBehavior<,>));
                    cfg.For<IMediator>().Use<Mediator>();
                });
        }

        return _container;
    }
}

//- - - - - - - - - - - - - - - - - - //

public record TestExceptionsRequest(string? Message) : IRequest<BasicResult>;

//- - - - - - - - - - - - - - - - - - //

public record TestParamaters(
    Func<Container> ContainerGenerator,
    Action<BasicResult> Challenger
);

//- - - - - - - - - - - - - - - - - - //
