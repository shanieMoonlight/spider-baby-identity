namespace ClArch.SimpleSpecification;
public static class SimpleSpecificationBuilder
{    
    public static IQueryable<T> BuildQuery<T>(this IQueryable<T> inputQuery, ASimpleSpecification<T> spec)
        where T : class
    {

        return spec.BuildQuery(inputQuery);
    }

}//Cls
