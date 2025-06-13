namespace CollectionHelpers;



/// <summary>
/// A Dictionary that uses case-insensitive string comparison for keys.
/// </summary>
/// <typeparam name="T">The type of values in the dictionary</typeparam>
public class InvariantStringDictionary<T> : Dictionary<string, T>
{
    /// <summary>
    /// Initializes a new instance with a custom string comparer.
    /// </summary>
    /// <param name="comparer">The comparer to use for string keys</param>
    /// <remarks>
    /// While any comparer can be provided, using a case-sensitive comparer defeats
    /// the purpose of this class.
    /// </remarks>
    public InvariantStringDictionary(IEqualityComparer<string> comparer)
          : base(comparer) { }



    /// <summary>
    /// Initializes a new instance using case-insensitive string comparison.
    /// </summary>
    public InvariantStringDictionary()
          : base(StringComparer.InvariantCultureIgnoreCase) { }



    /// <summary>
    /// Initializes a new instance with the specified capacity.
    /// </summary>
    /// <param name="capacity">Initial capacity of the dictionary</param>
    public InvariantStringDictionary(int capacity)
          : base(capacity, StringComparer.InvariantCultureIgnoreCase) { }



    /// <summary>
    /// Initializes a new instance from an existing dictionary.
    /// </summary>
    /// <param name="dictionary">Source dictionary to copy from</param>
    public InvariantStringDictionary(IDictionary<string, T> dictionary)
          : base(dictionary, StringComparer.InvariantCultureIgnoreCase) { }



    /// <summary>
    /// Initializes a new instance with initial values.
    /// </summary>
    /// <param name="initialValues">Collection of key-value pairs to add</param>
    public InvariantStringDictionary(IEnumerable<KeyValuePair<string, T>> initialValues)
          : base(StringComparer.InvariantCultureIgnoreCase)
    {
        if (initialValues != null)
        {
            foreach (var item in initialValues)
                this[item.Key] = item.Value;
        }
    }


}//Cls