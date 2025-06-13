using ID.Application.Mediatr.CqrsAbs;
using ID.Application.Utility.Enums;
using ID.Domain.Models;
using MyResults;

namespace ID.Application.Features.Account.Qry.GetProviders;
public class GetProvidersQryHandler() : IIdQueryHandler<GetProvidersQry, string[]>
{

    public Task<GenResult<string[]>> Handle(GetProvidersQry request, CancellationToken cancellationToken)
    {
        //Check if user exists
        var providers = MyEnums.GetDescriptions<TwoFactorProvider>();
        return Task.FromResult(GenResult<string[]>.Success([.. providers]));

    }

}//Cls
