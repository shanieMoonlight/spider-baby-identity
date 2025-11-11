using ID.Application.Features.Account.Cmd.Login;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Repos.Transactions;
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
        // Get the execution strategy from the transaction service and run the transactional unit inside it.
        var strategy = await transactionService.CreateExecutionStrategyAsync();

        return await strategy.ExecuteAsync(async ct =>
        {
            using var transaction = await transactionService.BeginTransactionAsync(ct);
            try
            {
                // Pass the token downstream if the RequestHandlerDelegate accepts it.
                var response = await next(ct);
                if (IdCommandTransactionPipelineBehaviour<TRequest, TResponse>.IsSuccessful(request, response))
                {
                    await transactionService.SaveChangesAsync(ct);
                    await transaction.CommitAsync(ct);
                }
                else
                {
                    await transaction.RollbackAsync(ct);
                }

                return response;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        }, cancellationToken);
    }

    //---------------------------------//

    //response.PreconditionRequired wil occur when Login succeeds but user Email has not been confirmed
    //We still want to save any changes. (Storing tokens etc...)
    private static bool IsSuccessful(TRequest request, TResponse response) =>
        response.Succeeded
        ||
        request is LoginCmd && response.PreconditionRequired;


}//Cls