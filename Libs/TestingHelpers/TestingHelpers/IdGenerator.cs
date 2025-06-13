namespace TestingHelpers;
public class IdGenerator
{

    public static int[] GetIntIds(int count = 20)
    {
        int[] numbers = new int[count];
        for (int i = 0; i < numbers.Length; i++)
            numbers[i] = i;

        return numbers;
    }

    //- - - - - - - - - - - - - - - - - - //

    public static List<int> GetIntIdsList(int count = 20) =>
        [.. GetIntIds(count)];

    //------------------------------------//

    public static Guid[] GetGuidIds(int count = 20)
    {
        Guid[] guids = new Guid[count];
        for (int i = 0; i < guids.Length; i++)
            guids[i] = Guid.NewGuid();

        return guids;
    }

    //- - - - - - - - - - - - - - - - - - //

    public static List<Guid> GetGuidIdsList(int count = 20) =>
        [.. GetGuidIds(count)];


}
