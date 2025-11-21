using ID.Application.Mediatr.Behaviours.Common;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using MyResults;

namespace ID.Application.Mediatr.Behaviours;


/// <summary>
/// Retruns NotFound response for all requests when not in Development environment
/// </summary>
public class DevModePipelineBehavior<TRequest, TResponse>(IWebHostEnvironment _env)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IIdDevModeRequest<AppUser>, IRequest<TResponse>
    where TResponse : BasicResult
{

    public  async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_env.IsDevelopment())
            return ResponseProvider.GenerateNotFoundResponse<TResponse>();

        return await next(cancellationToken);
    }


}//Cls