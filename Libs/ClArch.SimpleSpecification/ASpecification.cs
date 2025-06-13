using System.Linq.Expressions;
using CollectionHelpers;
using Microsoft.EntityFrameworkCore.Query;

namespace ClArch.SimpleSpecification;

/// <summary>  
/// Abstract base class for creating specifications for querying entities.  
/// </summary>  
/// <typeparam name="TEntity">The type of the entity.</typeparam>  
public abstract class ASimpleSpecification<TEntity>(Expression<Func<TEntity, bool>>? criteria = null)
   where TEntity : class
{
    /// <summary>  
    /// Gets or sets the criteria expression for the specification.  
    /// </summary>  
    private Expression<Func<TEntity, bool>> Criteria { get; set; } = criteria ??= (TEntity x) => true;

    /// <summary>  
    /// Gets the list of include expressions for the specification.  
    /// </summary>  
    private List<Func<IQueryable<TEntity>, IQueryable<TEntity>>> IncludeExpressions { get; } = [];

    /// <summary>  
    /// Gets or sets the order by expression for the specification.  
    /// </summary>  
    private Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? OrderByExpression { get; set; }

    /// <summary>
    /// Gets or sets the number of entities to take.
    /// </summary>
    protected int? Take { get; private set; }

    /// <summary>
    /// Gets or sets the number of entities to skip.
    /// </summary>
    protected int? Skip { get; private set; }

    /// <summary>  
    /// If this is true, the query will be short-circuited and the default value will be returned.  
    /// (Optional)  
    /// </summary>  
    private Func<bool>? _shortCircuit;

    //------------------------------------//   

    /// <summary>  
    /// Adds an order by expression to the specification.  
    /// </summary>  
    /// <param name="orderByExpression">The order by expression.</param>  
    protected void SetOrderBy(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderByExpression) =>
        OrderByExpression = orderByExpression;

    //- - - - - - - - - - - - - - - - - - //   

    /// <summary>  
    /// Adds an include expression to the specification.  
    /// </summary>  
    /// <typeparam name="TProperty">The type of the property to include.</typeparam>  
    /// <param name="includeDelegate">The include delegate.</param>  
    public void SetInclude<TProperty>(Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, TProperty>> includeDelegate) =>
        IncludeExpressions.Add(query => includeDelegate(query));

    //------------------------------------//

    /// <summary>
    /// Sets the number of entities to take.
    /// </summary>
    /// <param name="take">The number of entities to take.</param>
    public void SetTake(int take) =>
        Take = take > 0 ? take : null;

    //------------------------------------//

    /// <summary>
    /// Sets the number of entities to skip.
    /// </summary>
    /// <param name="skip">The number of entities to skip.</param>
    public void SetSkip(int skip) =>
        Skip = skip > 0 ? skip : null;

    //------------------------------------//   

    /// <summary>  
    /// Sets the short circuit function for the specification.  
    /// </summary>  
    /// <param name="shortCircuit">The short circuit function.</param>  
    public void SetShortCircuit(Func<bool> shortCircuit) => _shortCircuit = shortCircuit;

    /// <summary>  
    /// Determines whether the query should be short-circuited.  
    /// </summary>  
    /// <returns><c>true</c> if the query should be short-circuited; otherwise, <c>false</c>.</returns>  
    public bool ShouldShortCircuit() => _shortCircuit?.Invoke() ?? false;

    //------------------------------------//   

    /// <summary>  
    /// Builds the query based on the specification.  
    /// </summary>  
    /// <typeparam name="T">The type of the entity.</typeparam>  
    /// <param name="inputQuery">The input query.</param>  
    /// <returns>The query built based on the specification.</returns>  
    public IQueryable<TEntity> BuildQuery(IQueryable<TEntity> inputQuery)
    {
        var criteriaQry = ApplyCriteria(inputQuery, Criteria);
        var includeQry = ApplyIncludes(criteriaQry, IncludeExpressions);
        var orderQry = ApplyOrderBy(includeQry, OrderByExpression);
        var skipQry = ApplySkip(orderQry, Skip);
        var takeQry = ApplyTake(skipQry, Take);

        return takeQry;
    }

    //------------------------------------//   

    /// <summary>  
    /// Applies the criteria to the query.  
    /// </summary>  
    /// <typeparam name="T">The type of the entity.</typeparam>  
    /// <param name="query">The query.</param>  
    /// <param name="criteria">The criteria expression.</param>  
    /// <returns>The query with the criteria applied.</returns>  
    private static IQueryable<T> ApplyCriteria<T>(IQueryable<T> query, Expression<Func<T, bool>> criteria) where T : class
    {
        query = query.Where(criteria);
        return query;
    }

    //- - - - - - - - - - - - - - - - - - //  

    /// <summary>  
    /// Applies the include expressions to the query.  
    /// </summary>  
    /// <typeparam name="T">The type of the entity.</typeparam>  
    /// <param name="query">The query.</param>  
    /// <param name="includeExpressions">The include expressions.</param>  
    /// <returns>The query with the include expressions applied.</returns>  
    private static IQueryable<T> ApplyIncludes<T>(IQueryable<T> query, List<Func<IQueryable<T>, IQueryable<T>>> includeExpressions)
        where T : class
    {
        if (!includeExpressions.AnyValues())
            return query;

        foreach (var include in includeExpressions)
        {
            query = include(query);
        }

        return query;
    }

    //- - - - - - - - - - - - - - - - - - //  

    /// <summary>  
    /// Applies the order by expression to the query.  
    /// </summary>  
    /// <typeparam name="T">The type of the entity.</typeparam>  
    /// <param name="query">The query.</param>  
    /// <param name="orderByExpression">The order by expression.</param>  
    /// <returns>The query with the order by expression applied.</returns>  
    private static IQueryable<T> ApplyOrderBy<T>(IQueryable<T> query, Func<IQueryable<T>, IOrderedQueryable<T>>? orderByExpression)
        where T : class
    {
        if (orderByExpression is null)
            return query;

        query = orderByExpression(query);
        return query;
    }

    //- - - - - - - - - - - - - - - - - - //  

    /// <summary>
    /// Applies the skip value to the query.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="query">The query.</param>
    /// <param name="skip">The number of entities to skip.</param>
    /// <returns>The query with the skip value applied.</returns>
    private static IQueryable<T> ApplySkip<T>(IQueryable<T> query, int? skip)
        where T : class
    {
        if (skip.HasValue)
            query = query.Skip(skip.Value);

        return query;
    }

    //- - - - - - - - - - - - - - - - - - //  

    /// <summary>
    /// Applies the take value to the query.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="query">The query.</param>
    /// <param name="take">The number of entities to take.</param>
    /// <returns>The query with the take value applied.</returns>
    private static IQueryable<T> ApplyTake<T>(IQueryable<T> query, int? take)
        where T : class
    {
        if (take.HasValue)
            query = query.Take(take.Value);

        return query;
    }

    //------------------------------------//   

    #region Testing Helpers 

    // Add this protected method to expose the Criteria for testing
    public Expression<Func<TEntity, bool>> TESTING_GetCriteria() => Criteria;


    /// <summary>
    /// Compares the criteria of this specification with another specification for a given entity.
    /// </summary>
    /// <param name="that">The other specification to compare with.</param>
    /// <param name="entity">The entity to test the criteria against.</param>
    /// <returns>True if both criteria evaluate to the same result for the entity; otherwise, false.</returns>
    public bool TESTING_CompareCriteria(ASimpleSpecification<TEntity> that, TEntity entity)
    {
        //var one = this.TESTING_GetCriteria().Compile().Invoke(entity);
        //var two = that.TESTING_GetCriteria().Compile().Invoke(entity);
        return that != null
        &&
        this.TESTING_GetCriteria().Compile().Invoke(entity) == that.TESTING_GetCriteria().Compile().Invoke(entity);
    }

    #endregion

    //------------------------------------//   


}//Cls

