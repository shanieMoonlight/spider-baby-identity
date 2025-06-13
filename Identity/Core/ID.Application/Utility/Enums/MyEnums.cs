namespace ID.Application.Utility.Enums;

/// <summary>
/// Utility class for working with enum types and their descriptions.
/// </summary>
public class MyEnums
{

    //---------------------------------//
    /// <summary>
    /// Gets a list of description strings for all values in the specified enum type.
    /// </summary>
    /// <typeparam name="TEnum">The enum type to process</typeparam>
    /// <returns>List of non-null descriptions for enum values</returns>
    public static List<string> GetDescriptions<TEnum>() where TEnum : struct, Enum
    {
        IEnumerable<string?> unfiltered =  Enum.GetValues<TEnum>().Select(x => x.GetDescription());
        List<string> filtered = [];
        foreach (var item in unfiltered)
        {
            if (item != null)
                filtered.Add(item);            
        }
        return filtered;
    }

    //---------------------------------//

    /// <summary>
    /// Gets the minimum integer value of the specified enum type.
    /// </summary>
    /// <typeparam name="TEnum">The enum type to process</typeparam>
    /// <returns>The minimum integer value of the enum, or 0 if the enum has no values</returns>
    public static int GetMinEnumValue<TEnum>() where TEnum : Enum =>
        GetMinEnum<TEnum>()
            ?.ToInt()
            ?? 0;

    //- - - - - - - - - - - - - - - - -//

    /// <summary>
    /// Gets the minimum value of the specified enum type.
    /// </summary>
    /// <typeparam name="TEnum">The enum type to process</typeparam>
    /// <returns>The minimum value of the enum</returns>
    public static TEnum GetMinEnum<TEnum>() where TEnum : Enum =>
        Enum.GetValues(typeof(TEnum))
            .Cast<TEnum>()
            .Min()!;

    //---------------------------------//

    /// <summary>
    /// Gets the maximum integer value of the specified enum type.
    /// </summary>
    /// <typeparam name="TEnum">The enum type to process</typeparam>
    /// <returns>The maximum integer value of the enum, or 0 if the enum has no values</returns>
    public static int GetMaxEnumValue<TEnum>() where TEnum : Enum =>
        GetMaxEnum<TEnum>()
            ?.ToInt()
            ?? 0;

    //- - - - - - - - - - - - - - - - -//

    /// <summary>
    /// Gets the maximum value of the specified enum type.
    /// </summary>
    /// <typeparam name="TEnum">The enum type to process</typeparam>
    /// <returns>The maximum value of the enum</returns>
    public static TEnum GetMaxEnum<TEnum>() where TEnum : Enum =>
        Enum.GetValues(typeof(TEnum))
            .Cast<TEnum>()
            .Max()!;


}//Cls
