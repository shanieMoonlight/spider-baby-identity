namespace MyResults;
public static class BasicResultExtensions
{
    /// <summary>
    /// Converts this result in to the <typeparamref name="TNew"/> Equivilant
    /// Everything is the same except the value
    /// </summary>
    /// <typeparam name="TNew">New type</typeparam>
    /// <param name="newValue">New value if one exists</param>
    /// <returns>Result of type <typeparamref name="TNew"/></returns>
    public static GenResult<TNew> Convert<TNew>(this BasicResult basicResult, TNew? newValue = default) =>
        basicResult.Succeeded
        ? GenResult<TNew>.Success(newValue!, basicResult.Info)
        : GenResult<TNew>.Failure(basicResult.Status, basicResult.Exception, basicResult.Info);
}
