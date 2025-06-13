namespace CollectionHelpers;

/// <summary>
/// Provides extension methods for IEnumerable and ICollection types.
/// </summary>
public static class IEnumerableExtensions
{
    /// <summary>
    /// Adds a range of elements to an IEnumerable.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="items">The original collection.</param>
    /// <param name="range">The collection of elements to add.</param>
    /// <returns>A new IEnumerable containing the original elements and the added range.</returns>
    public static IEnumerable<T> AddRange<T>(this IEnumerable<T> items, IEnumerable<T> range)
    {
        if (!range.AnyValues())
            return items;

        items ??= [];

        return items.Concat(range);
    }


    //------------------------------//


    /// <summary>
    /// Adds a range of elements to an ICollection.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="items">The original collection.</param>
    /// <param name="range">The collection of elements to add.</param>
    /// <returns>The original ICollection with the added range.</returns>
    public static ICollection<T> AddRange<T>(this ICollection<T> items, IEnumerable<T> range)
    {
        if (!range.AnyValues())
            return items;

        items ??= [];

        foreach (var item in range)
        {
            items.Add(item);
        }

        return items;
    }

    //------------------------------//

    /// <summary>
    /// Checks if <paramref name="enumerable"/> is non-null and has at least 1 element.
    /// </summary>
    /// <typeparam name="T">Type of elements in <paramref name="enumerable"/>.</typeparam>
    /// <param name="enumerable">Collection of T's.</param>
    /// <returns>Whether <paramref name="enumerable"/> has any items.</returns>
    public static bool AnyValues<T>(this List<T>? enumerable) =>
        enumerable != null && enumerable.Count != 0;

    //------------------------------//

    /// <summary>
    /// Checks if <paramref name="array"/> is non-null and has at least 1 element.
    /// </summary>
    /// <typeparam name="T">Type of elements in <paramref name="array"/>.</typeparam>
    /// <param name="array">Collection of T's.</param>
    /// <returns>Whether <paramref name="array"/> has any items.</returns>
    public static bool AnyValues<T>(this T[]? array) =>
        array != null && array.Length != 0;

    //------------------------------//

    /// <summary>
    /// Checks if <paramref name="enumerable"/> is non-null and has at least 1 element.
    /// </summary>
    /// <typeparam name="T">Type of elements in <paramref name="enumerable"/>.</typeparam>
    /// <param name="enumerable">Collection of T's.</param>
    /// <returns>Whether <paramref name="enumerable"/> has any items.</returns>
    public static bool AnyValues<T>(this IEnumerable<T>? enumerable) =>
        enumerable != null && enumerable.Any();

    //------------------------------//

    /// <summary>
    /// Converts an IEnumerable of strings to lowercase.
    /// </summary>
    /// <param name="strings">The original collection of strings.</param>
    /// <returns>A new IEnumerable with all strings in lowercase.</returns>
    public static IEnumerable<string> ToLower(this IEnumerable<string> strings) =>
        strings?.Select(s => s.ToLower()) ?? [];

    //------------------------------//

    /// <summary>
    /// Converts an IEnumerable of strings to a List with all strings in lowercase.
    /// </summary>
    /// <param name="strings">The original collection of strings.</param>
    /// <returns>A new List with all strings in lowercase.</returns>
    public static List<string> ToLowerList(this IEnumerable<string> strings) =>
        [.. strings.ToLower()];

    //------------------------------//

    /// <summary>
    /// Converts a collection of items to a string that is separated by a specified separator.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="items">The original collection.</param>
    /// <param name="separator">The separator string.</param>
    /// <returns>A string with the elements separated by the specified separator.</returns>
    public static string ToSeparatedString<T>(this IEnumerable<T> items, string separator = ",")
        => string.Join(separator, items);

    //------------------------------//

    /// <summary>
    /// Combines two lists, replacing any null list with an empty list to avoid exceptions.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collections.</typeparam>
    /// <param name="list">The first collection of elements.</param>
    /// <param name="otherList">The second collection of elements.</param>
    /// <returns>A new IEnumerable containing the combined elements of both lists.</returns>
    public static IEnumerable<T> UnionSafe<T>(this IEnumerable<T> list, IEnumerable<T> otherList) =>
        (list ?? [])
            .Union(otherList ?? []);

    //------------------------------//

    /// <summary>
    /// Joins the elements of a collection into a string, separated by a specified separator.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <typeparam name="TResult">The type of the value returned by the selector function.</typeparam>
    /// <param name="list">The original collection.</param>
    /// <param name="separator">The separator string.</param>
    /// <param name="selector">A function to project each element and its index into a new form.</param>
    /// <returns>A string with the elements separated by the specified separator.</returns>
    public static string JoinStr<T, TResult>(this IEnumerable<T> list, string separator, Func<T, TResult>? selector) =>
      selector is null 
        ? JoinStr(list, separator)
        : string.Join(separator, list.Select(selector));

    //- - - - - - - - - - - - - - - //

    /// <summary>
    /// Joins the elements of a collection into a string, separated by a specified separator.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <typeparam name="TResult">The type of the value returned by the selector function.</typeparam>
    /// <param name="list">The original collection.</param>
    /// <param name="separator">The separator string.</param>
    /// <param name="selector">A function to project each element and its index into a new form.</param>
    /// <returns>A string with the elements separated by the specified separator.</returns>
    public static string JoinStr<T, TResult>(this IEnumerable<T> list, char separator, Func<T, int, TResult>? selector) =>
      selector is null
        ? JoinStr(list, separator)
        : string.Join(separator, list.Select(selector));

    //- - - - - - - - - - - - - - - //

    /// <summary>
    /// Joins the elements of a collection into a string, separated by a specified separator.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="list">The original collection.</param>
    /// <param name="separator">The separator string.</param>
    /// <returns>A string with the elements separated by the specified separator.</returns>
    public static string JoinStr<T>(this IEnumerable<T> list, string separator) =>
      string.Join(separator, list);

    //- - - - - - - - - - - - - - - //

    /// <summary>
    /// Joins the elements of a collection into a string, separated by a specified separator.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="list">The original collection.</param>
    /// <param name="separator">The separator character.</param>
    /// <returns>A string with the elements separated by the specified separator.</returns>
    public static string JoinStr<T>(this IEnumerable<T> list, char separator) =>
      string.Join(separator, list);

    //------------------------------//

    /// <summary>
    /// Converts an IEnumerable to a List, replacing any null collection with an empty list to avoid exceptions.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="enumerable">The original collection.</param>
    /// <returns>A new List containing the elements of the original collection.</returns>
    public static List<T> ToSafeList<T>(this IEnumerable<T>? enumerable) =>
       enumerable?.ToList() ?? [];

    //------------------------------//

    /// <summary>
    /// Converts an IEnumerable to an array, replacing any null collection with an empty array to avoid exceptions.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="enumerable">The original collection.</param>
    /// <returns>A new array containing the elements of the original collection.</returns>
    public static T[] ToSafeArray<T>(this IEnumerable<T>? enumerable) =>
       enumerable?.ToArray() ?? [];


}//Cls
