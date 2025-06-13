using ControllerHelpers;
using ID.Application.Features.OutboxMessages;
using ID.Application.Features.OutboxMessages.Qry.GetAll;
using ID.Application.Features.OutboxMessages.Qry.GetAllByType;
using ID.Application.Features.OutboxMessages.Qry.GetById;
using ID.Application.Features.OutboxMessages.Qry.GetPage;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ID.GlobalSettings.Routes;
using Pagination;

namespace ID.Presentation.Controllers;

[ApiController]
[Route($"{IdRoutes.Base}/[controller]/[action]")]
//[MntcMinimumAuthenticator.ActionFilter]
public class IdOutboxMessagesController(ISender sender) : Controller
{
    //------------------------//

    [HttpGet]
    public async Task<ActionResult<IEnumerable<IdOutboxMessageDto>>> GetAll() =>
        this.ProcessResult(await sender.Send(new GetAllOutboxMessagesQry()));

    //------------------------//

    /// <summary>
    /// Gets the OutboxMessage with Id = <paramref name="id"/> 
    /// </summary>
    /// <returns>The OutboxMessage matching the id or NotFound</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<IdOutboxMessageDto>> Get(Guid id) =>
        this.ProcessResult(await sender.Send(new GetOutboxMessageByIdQry(id)));

    //------------------------//

    /// <summary>
    /// Gets the OutboxMessage with Name = <paramref name="name"/> 
    /// </summary>
    /// <returns>The OutboxMessage matching the id or NotFound</returns>
    [HttpGet("{name}")]
    public async Task<ActionResult<IEnumerable<IdOutboxMessageDto>>> GetAllByType(string? name) =>
        this.ProcessResult(await sender.Send(new GetAllOutboxMessagesByTypeQry(name)));

    //------------------------//

    /// <summary>
    /// Gets a paginated list of OutboxMessages
    /// </summary>
    /// <param name="request">Filtering and Sorting Info</param>
    /// <returns>Paginated list of OutboxMessages</returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResponse<IdOutboxMessageDto>>> Page([FromBody] PagedRequest? request) =>
        this.ProcessResult(await sender.Send(new GetOutboxMessagePageQry(request)));

    //------------------------//

} //Cls

