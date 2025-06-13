namespace MyResults;

public static class GrExtentions
{

    /// <summary>
    /// Null or Failed
    /// </summary>
    /// <returns></returns>
    public static bool IsFalsey<T>(this GenResult<T>? result) => result == null || !result.Succeeded;

}//Cls
