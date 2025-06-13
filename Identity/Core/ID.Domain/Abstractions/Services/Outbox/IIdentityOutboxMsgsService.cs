using ID.Domain.Entities.OutboxMessages;
using Pagination;

namespace ID.Domain.Abstractions.Services.Outbox;
public interface IIdentityOutboxMsgsService
{
    //- - - - - - - - - - - - //

    /// <summary>
    /// Get all IdOutboxMessages.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of all IdOutboxMessages.</returns>
    Task<IReadOnlyList<IdOutboxMessage>> GetAllAsync();

    //- - - - - - - - - - - - //

    /// <summary>
    /// Get the IdOutboxMessage with id, <paramref name="id"/> . Incluse no Foreign Key Properties.
    /// </summary>
    /// <param name="id">Entity identifier</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the IdOutboxMessage if found, otherwise null.</returns>
    Task<IdOutboxMessage?> GetByIdAsync(Guid? id);

    //- - - - - - - - - - - - //

    Task<IReadOnlyList<IdOutboxMessage>> GetAllByTypeAsync(string? name);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Retrieves a paginated list of IdOutboxMessages based on the provided request.
    /// </summary>
    /// <param name="request">The paginated request containing page number, page size, sorting, and filtering criteria.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a paginated list of IdOutboxMessages.</returns>
    Task<Page<IdOutboxMessage>> GetPageAsync(PagedRequest request);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Retrieves a paginated list of IdOutboxMessages.
    /// </summary>
    /// <param name="pageNumber">The number of the page to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="sortList">A collection of sorting criteria.</param>
    /// <param name="filterList">A collection of filtering criteria (optional).</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a paginated list of IdOutboxMessages.</returns>
    Task<Page<IdOutboxMessage>> GetPageAsync(int pageNumber, int pageSize, IEnumerable<SortRequest> sortList, IEnumerable<FilterRequest>? filterList = null);

    //- - - - - - - - - - - - //

}//Int
