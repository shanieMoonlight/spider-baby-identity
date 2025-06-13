using ClArch.SimpleSpecification;
using ID.Domain.Entities.Common;

namespace ID.Infrastructure.Persistance.Abstractions.Repos.Specs;

/// <summary>
/// Specification for querying an entity by its ID, with no noavigation properties.
/// Inherit form this to create a "ByIdWithDetails" Specification
/// <para>Will short circuit if the ID is null.</para> 
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public class GetByIdSpec<TEntity> : ASimpleSpecification<TEntity> where TEntity : class, IIdBaseDomainEntity
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetByIdSpec{TEntity}"/> class.
    /// <para>Will short circuit if the ID is null.</para> 
    /// </summary>
    /// <param name="id">The ID of the entity to query.</param>
    internal GetByIdSpec(Guid? id) : base(tm => tm.Id == id)
    {
        SetShortCircuit(() => !id.HasValue);
    }
}
