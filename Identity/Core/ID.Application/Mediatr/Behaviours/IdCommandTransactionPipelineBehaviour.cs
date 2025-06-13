using ID.Application.Features.Account.Cmd.Login;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.Transactions;
using MediatR;
using MyResults;

namespace ID.Application.Mediatr.Behaviours;
public sealed class IdCommandTransactionPipelineBehaviour<TRequest, TResponse>(IIdentityTransactionService transactionService)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseIdCommand
    where TResponse : BasicResult
{
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        using var transaction = await transactionService.BeginTransactionAsync(cancellationToken);
        try
        {
            var response = await next(cancellationToken);
            if (IdCommandTransactionPipelineBehaviour<TRequest, TResponse>.IsSuccessful(request, response))
            {
                await transactionService.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            else
            {
                await transaction.RollbackAsync(cancellationToken);
            }

            return response;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    //---------------------------------//

    //response.PreconditionRequired wil occur when Login succeeds but user Email has not been confirmed
    //We still want to save any changes. (Storing tokens etc...)
    private static bool IsSuccessful(TRequest request, TResponse response) =>
        response.Succeeded
        ||
        request is LoginCmd && response.PreconditionRequired;

    //---------------------------------//

}//Cls