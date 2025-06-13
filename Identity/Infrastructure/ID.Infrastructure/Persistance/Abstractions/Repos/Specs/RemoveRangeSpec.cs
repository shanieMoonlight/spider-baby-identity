using ClArch.SimpleSpecification;
using ID.Domain.Entities.Common;
using System.Linq.Expressions;

namespace ID.Infrastructure.Persistance.Abstractions.Repos.Specs;

/// <summary>
/// Specification for removing a range of entities based on a criteria.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <param name="criteria">The criteria expression to filter entities to be removed.</param>
internal class RemoveRangeSpec<TEntity>(Expression<Func<TEntity, bool>>? criteria)
    : ASimpleSpecification<TEntity>(criteria) where TEntity : class, IIdBaseDomainEntity
{ }
