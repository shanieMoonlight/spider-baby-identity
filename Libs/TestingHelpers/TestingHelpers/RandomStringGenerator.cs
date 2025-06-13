using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;

namespace TestingHelpers;
public static class RandomStringGenerator
{
    private static readonly Random _random = new();


    //------------------------------------//

    public static string Generate(int maxLength = 20)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        return new string(Characters(chars, maxLength));
    }

    //------------------------------------//

    private static char[] Characters(string chars, int count)
    {
        var charsArray = chars.ToCharArray();
        return Enumerable.Range(0, count)
                         .Select(_ => charsArray[_random.Next(charsArray.Length)])
                         .ToArray();
    }

    //------------------------------------//

    public static string Word(int min = 4, int max = 10) =>
        RandomizerFactory
            .GetRandomizer(new FieldOptionsTextWords() { Min = min, Max = max })
            ?.Generate()
            ?.Split(' ')
            ?.FirstOrDefault()
            ?? Generate();

    //------------------------------------//

    public static string Sentence(int min = 4, int max = 10) =>
        RandomizerFactory
            .GetRandomizer(new FieldOptionsTextWords() { Min = min, Max = max })
            ?.Generate()
            ?? Generate();

    //------------------------------------//

    public static string FirstName()
    {
        // Generate a random word
        var randomizerFirstName = RandomizerFactory.GetRandomizer(new FieldOptionsFirstName());
        return randomizerFirstName?.Generate() ?? Generate();
    }

    //------------------------------------//

    public static string LastName()
    {
        // Generate a random word
        var randomizerLastName = RandomizerFactory.GetRandomizer(new FieldOptionsLastName());
        return randomizerLastName?.Generate() ?? Generate();
    }

    //------------------------------------//

}//Cls