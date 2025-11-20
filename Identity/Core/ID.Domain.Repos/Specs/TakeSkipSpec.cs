using ClArch.SimpleSpecification;
using ID.Domain.Entities.Common;
using System.Linq.Expressions;

namespace ID.Domain.Repos.Specs;

/// <summary>
/// Specification for querying an entity with optional criteria.
/// Inherit from this to create a "TakeSkip" Specification.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="TakeSkipSpec{TEntity}"/> class.
/// </remarks>
internal class TakeSkipSpec<TEntity> : ASimpleSpecification<TEntity> where TEntity : class, IIdBaseDomainEntity
{
    public int Count { get; private set; }

    /// <param name="criteria">The criteria expression for the specification.</param>
    public TakeSkipSpec(
        int count,
        int skip,
        Expression<Func<TEntity, bool>>? criteria) : base(criteria)
    {
        Count = count;
        SetSkip(skip);
    }
}
