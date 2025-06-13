using ClArch.SimpleSpecification;
using ID.Domain.Entities.Common;
using System.Linq.Expressions;

namespace ID.Infrastructure.Persistance.Abstractions.Repos.Specs;

/// <summary>
/// Specification for querying an entity with optional criteria.
/// Inherit from this to create a "TakeSkip" Specification.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="TakeSkipSpec{TEntity}"/> class.
/// </remarks>
/// <param name="criteria">The criteria expression for the specification.</param>
internal class TakeSkipSpec<TEntity>(
    int count,
    int skip,
    Expression<Func<TEntity, bool>>? criteria)
    : ASimpleSpecification<TEntity>(criteria) where TEntity : class, IIdBaseDomainEntity
{
    public int Count { get; private set; } = count;
    public int Skip { get; private set; } = skip;
}
