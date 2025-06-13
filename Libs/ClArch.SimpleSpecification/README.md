# ClArch.SimpleSpecification

This library provides a simple implementation of the Specification pattern in .NET 8.



## Installation
To install ClArch.SimpleSpecification, import it into your project:



## Usage

Here's a basic example of how to use the ClArch.SimpleSpecification library:

    public virtual async Task<IReadOnlyList<T>> ListAllAsync(ASimpleSpecification<T> spec, CancellationToken cancellationToken = default)
    {
        if (spec.ShouldShortCircuit())
            return [];

        return await Db.Set<T>()
            .BuildQuery(spec)
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public virtual async Task<T?> FirstOrDefaultAsync(ASimpleSpecification<T> spec, CancellationToken cancellationToken = default)
    {
        if (spec.ShouldShortCircuit())
            return null;

        return await Db.Set<T>()
            .BuildQuery(spec)
            .FirstOrDefaultAsync(cancellationToken);
    }


Here's a basic example of how to create a new Spcification:

    /// <summary>
    /// Specification for querying IdOutboxMessages by type.
    /// </summary>
    internal class OutboxMsgsByTypeSpec : ASimpleSpecification<IdOutboxMessage>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OutboxMsgsByTypeSpec"/> class.
        /// </summary>
        /// <param name="type">The type to query by.</param>
        public OutboxMsgsByTypeSpec(string? type)
            : base(e => e.Type.Contains(type!))
        {
            SetShortCircuit(() => string.IsNullOrWhiteSpace(type));
        }
    }




    Generic:/// <summary>
    /// Specification for querying an entity by its ID, with no noavigation properties.
    /// Inherit form this to create a "ByIdWithDetails" Specification
    /// <para>Will short circuit if the ID is null.</para> 
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    internal class GetByIdSpec<TEntity> : ASimpleSpecification<TEntity> where TEntity : class, IIdBaseDomainEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetByIdSpec{TEntity}"/> class.
        /// <para>Will short circuit if the ID is null.</para> 
        /// </summary>
        /// <param name="id">The ID of the entity to query.</param>
        public GetByIdSpec(Guid? id) : base(tm => tm.Id == id)
        {
            SetShortCircuit(() => !id.HasValue);
        }
    }



## Warning

This will "leak" the query. So in a Clean Architecture or DDD project, you should probably only use this in the Infrastructure layer.
If you have a scenario where the Repositories are defined and implemented in the Infrastructure layer, then this is a good fit.


Options for if you are accessing Repositories from the Application layer: 
    Create an ISpecFactory interface. 
    Implement the ISpecFactory in the Infrastructure layer.

    Example:
    Application layer:
    public interface IMySpecificationFactory   
    {
        GetByIdSpec<T> GetByIdSpec<T>(Guid? id) where T : class, IIdBaseDomainEntity;
        OutboxMsgsCompletedOlderThanSpec OutboxMsgsCompletedOlderThanSpec(int days = 14);
    }

    
    Infrastructure layer:
    internal class MySpecificationFactory : IMySpecificationFactory
    {

        public GetByIdSpec<T> GetByIdSpec<T>(Guid? id) where T : class, IIdBaseDomainEntity => new(id);
        public OutboxMsgsCompletedOlderThanSpec OutboxMsgsCompletedOlderThanSpec(int days = 14) => new(days);
        //etc...
    }


    Make constructors internal     
    /// <summary>
    /// Specification for all completed outbox messages older than the specified number of days.
    /// </summary>
    public class OutboxMsgsCompletedOlderThanSpec : ASimpleSpecification<IdOutboxMessage>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="OutboxMsgsByTypeSpec"/> class.
        /// </summary>
        /// <param name="days">The number of days to look back. Default is 14 days.</param>
        internal OutboxMsgsCompletedOlderThanSpec(int days = 14)
            : base(om =>
                om.CreatedOnUtc < DateTime.Now.AddDays(-days)
                && om.ProcessedOnUtc != null
                && string.IsNullOrWhiteSpace(om.Error)
            )
        {
            SetShortCircuit(() => days < 0);
        }
    }

    //Hook it up
    services.TryAddScoped<IMySpecificationFactory, MySpecificationFactory>();



If this is too much effort try Ardalis.Specification on GitHub



## License

This project is licensed under the MIT License.
