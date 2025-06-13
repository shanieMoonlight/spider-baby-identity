namespace CollectionHelpers;

public static class DictionaryExtensions
{

    /// <summary>
    /// Checks if <paramref name="dictionary"/> is non-null and has at least 1 element
    /// </summary>
    /// <typeparam name="T">Type of elements in <paramref name="dictionary"/></typeparam>
    /// <param name="dictionary">Collection of T's</param>
    /// <returns>Whether <paramref name="dictionary"/> has any items</returns>
    public static bool AnyValues<TKey, TVal>(this IReadOnlyDictionary<TKey, TVal>? dictionary) where TKey : notnull =>
        dictionary != null 
        && dictionary.Count != 0;


    //- - - - - - - - - - - - - - - //


    /// <summary>
    /// Determines whether the System.Collections.Generic.IDictionary`2 contains an element
    //     with the specified key.  Handles nulls safely.
    /// </summary>
    /// <param name="dictionary"></param>
    /// <param name="key">The key to locate in the System.Collections.Generic.IDictionary`2.</param>
    /// <returns></returns>
    public static bool ContainsKeySafe<TKey, TVal>(this IDictionary<TKey, TVal>? dictionary, TKey? key) where TKey : notnull =>
        key != null 
        && dictionary != null 
        && dictionary.ContainsKey(key);


}
