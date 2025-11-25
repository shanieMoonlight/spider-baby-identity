namespace ID.Application.Features.Account.TrustedDevices.Qry.GetPage;
internal class GetTrustedDevicesPageQryHandler() : IIdQueryHandler<GetTrustedDevicesPageQry, PagedResponse<TrustedDeviceDto>>
{

    public Task<GenResult<PagedResponse<TrustedDeviceDto>>> Handle(GetTrustedDevicesPageQry request, CancellationToken cancellationToken)
    {
        var pgRequest = request.PagedRequest ?? PagedRequest.Empty();

        var pgNumber = pgRequest.PageNumber;
        var pgSize = pgRequest.PageSize;
        var sortList = pgRequest.SortList;
        var filterList = pgRequest.FilterList;

        var user = request.PrincipalUser;
        var trustedDevices = user.TrustedDevices;

        var pageData = trustedDevices
            .Skip((pgNumber - 1) * pgSize)
            .Take(pgSize)
            .Select(td => td.ToDto())
            .ToList();

        var page = new Page<TrustedDeviceDto>(pageData, pgNumber, pgSize);
        var response = new PagedResponse<TrustedDeviceDto>(page, pgRequest);

        return Task.FromResult(GenResult<PagedResponse<TrustedDeviceDto>>.Success(response));

    }


}//Cls
