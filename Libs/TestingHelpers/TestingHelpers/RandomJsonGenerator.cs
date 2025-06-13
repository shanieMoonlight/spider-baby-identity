using Newtonsoft.Json;

namespace TestingHelpers;
public class RandomJsonGenerator
{
    private static readonly Random _random = new();

    //-------------------------------------//

    public static string Generate(int count)
    {
        var jsonLines = new List<string>();

        for (int i = 0; i < count; i++)
        {
            var jsonObject = new Dictionary<string, object>
            {
                { "id", _random.Next(1000000) },
                { "name", $"Item {_random.Next(100)}" },
                { "price", _random.NextDouble() * 100 },
                { "category", GetRandomCategory() },
                { "tags", GetRandomTags() }
            };

            jsonLines.Add(JsonConvert.SerializeObject(jsonObject));
        }

        return string.Join(Environment.NewLine, jsonLines);
    }

    //-------------------------------------//

    private static string GetRandomCategory()
    {
        var categories = new[] { "Electronics", "Books", "Clothing", "Home Goods", "Sports Equipment" };
        return categories[_random.Next(categories.Length)];
    }

    //-------------------------------------//

    private static List<string> GetRandomTags()
    {
        var tags = new[] { "New", "Sale", "Discounted", "Popular", "Best Seller" };
        return Enumerable.Range(0, _random.Next(3, 6))
                         .Select(i => tags[_random.Next(tags.Length)])
                         .ToList();
    }

    //-------------------------------------//

}//Cls