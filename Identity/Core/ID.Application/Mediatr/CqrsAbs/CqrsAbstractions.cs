using MediatR;
using MyResults;

namespace ID.Application.Mediatr.CqrsAbs;

//-----------------------------------//

/// <summary>
/// Use to identify all types of Command requests.
/// Used by Mediatr pipelines etc.
/// </summary>
public interface IBaseIdCommand;
public interface IIdCommand : IRequest<BasicResult>, IIdPrincipalInfoRequest, IBaseIdCommand;
public interface IIdCommand<TResponse> : IRequest<GenResult<TResponse>>, IIdPrincipalInfoRequest, IBaseIdCommand;

//-----------------------------------//

public interface IIdCommandHandler<TCommand> : IRequestHandler<TCommand, BasicResult>
    where TCommand : IIdCommand;
public interface IIdCommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, GenResult<TResponse>>
    where TCommand : IIdCommand<TResponse>;

//-----------------------------------//

public interface IBaseIdQuery;
public interface IIdQuery : IRequest<BasicResult>, IIdPrincipalInfoRequest, IBaseIdQuery;
public interface IIdQuery<TResponse> : IRequest<GenResult<TResponse>>, IIdPrincipalInfoRequest, IBaseIdQuery;

//-----------------------------------//

/// <summary>
/// Use to identify all types of Query requests.
/// Used by Mediatr pipelines etc.
/// </summary>
public interface IIdQueryHandler<TQuery> : IRequestHandler<TQuery, BasicResult>
    where TQuery : IIdQuery;
public interface IIdQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, GenResult<TResponse>>
    where TQuery : IIdQuery<TResponse>;


//-----------------------------------//

