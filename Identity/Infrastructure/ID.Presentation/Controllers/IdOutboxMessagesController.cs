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

    /// <summary>
    /// Retrieves all outbox messages in the system.
    /// </summary>
    /// <returns>A list of all outbox messages.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<IdOutboxMessageDto>>> GetAll() =>
        this.ProcessResult(await sender.Send(new GetAllOutboxMessagesQry()));

    //------------------------//

    /// <summary>
    /// Gets a specific outbox message by its unique ID.
    /// </summary>
    /// <param name="id">The ID of the outbox message.</param>
    /// <returns>The outbox message matching the ID, or NotFound.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<IdOutboxMessageDto>> Get(Guid id) =>
        this.ProcessResult(await sender.Send(new GetOutboxMessageByIdQry(id)));

    //------------------------//

    /// <summary>
    /// Gets all outbox messages of a specific type.
    /// </summary>
    /// <param name="name">The type or name to filter by.</param>
    /// <returns>A list of outbox messages matching the type.</returns>
    [HttpGet("{name}")]
    public async Task<ActionResult<IEnumerable<IdOutboxMessageDto>>> GetAllByType(string? name) =>
        this.ProcessResult(await sender.Send(new GetAllOutboxMessagesByTypeQry(name)));

    //------------------------//

    /// <summary>
    /// Gets a paginated list of outbox messages.
    /// </summary>
    /// <param name="request">Filtering and sorting information.</param>
    /// <returns>A paginated list of outbox messages.</returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResponse<IdOutboxMessageDto>>> Page([FromBody] PagedRequest? request) =>
        this.ProcessResult(await sender.Send(new GetOutboxMessagePageQry(request)));

    //------------------------//

} //Cls

