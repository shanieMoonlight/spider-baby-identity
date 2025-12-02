using ControllerHelpers;
using ID.Application.Features.IdRefreshTokens;
using ID.Application.Features.IdRefreshTokens.Qry.GetAll;
using ID.Application.Features.IdRefreshTokens.Qry.GetById;
using ID.Application.Features.IdRefreshTokens.Qry.GetPage;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pagination;

namespace ID.Presentation.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class IdRefreshTokensController(ISender sender) : Controller
{


    [HttpGet]
    public async Task<ActionResult<IdRefreshTokenDto[]>> GetAll() =>
        this.ProcessResult(await sender.Send(new GetAllIdRefreshTokensQry()));

    //--------------------------// 

    /// <summary>
    /// Gets the IdRefreshToken with Id = <paramref name="id"/> 
    /// </summary>
    /// <returns>The IdRefreshToken matching the id or NotFound</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<IdRefreshTokenDto>> Get(Guid id) =>
        this.ProcessResult(await sender.Send(new GetIdRefreshTokenByIdQry(id)));

    //--------------------------// 

    /// <summary>
    /// Gets a paginated list of IdRefreshTokens
    /// </summary>
    /// <param name="request">Filtering and Sorting Info</param>
    /// <returns>Paginated list of IdRefreshTokens</returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResponse<IdRefreshTokenDto>>> Page(PagedRequest? request) =>
        this.ProcessResult(await sender.Send(new GetIdRefreshTokensPageQry(request)));


} //Cls