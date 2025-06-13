using System;


namespace TestingHelpers;

public static class MyRandomDataGenerator
{

    //------------------------------------//

    private static readonly string[] FirstNames =
    [
        "John", "Jane", "Michael", "Sarah", "William", "Emily",
        "James", "Lisa", "Robert", "Amy", "Richard", "Elizabeth",
        "Thomas", "Jennifer", "Joseph", "Susan", "Charles", "Margaret",
        "Christopher", "Melissa", "Matthew", "Nicole", "Brian", "Rebecca",
        "Kevin", "Amy", "Stephen", "Sarah", "Joshua", "Elizabeth"
    ];

    private static readonly string[] LastNames =
    [
        "Smith", "Johnson", "Brown", "Davis", "Miller", "Wilson",
        "Anderson", "Thomas", "Jackson", "White", "Harris", "Martin",
        "Thompson", "Garcia", "Martinez", "Robinson", "Clark", "Rodriguez",
        "Lewis", "Lee", "Walker", "Hall", "Allen", "Young", "Hernandez",
        "King", "Wright", "Lopez", "Hill", "Scott", "Green", "Adams", "Baker"
    ];

    private static readonly string[] Domains =
    [
        "@gmail.com", "@yahoo.com", "@hotmail.com", "@outlook.com",
        "@aol.com", "@comcast.net", "@verizon.net", "@att.net", "@sbcglobal.net",
        "@earthlink.net", "@charter.net", "@cox.net", "@frontier.com",
        "@bellsouth.net", "@centurylink.net", "@juno.com", "@prodigy.net",
        "@netzero.com", "@msn.com", "@aim.com", "@comcast.com", "@yahoo.com"
    ];

    private static readonly string[] PhonePrefixes =
    [
        "(123)", "(456)", "(789)", "(098)", "(234)", "(567)",
        "(890)", "(101)", "(112)", "(113)", "(114)", "(115)",
        "(116)", "(117)", "(118)", "(119)", "(120)", "(121)",
        "(122)", "(123)", "(124)", "(125)", "(126)", "(127)"
    ];

    //------------------------------------//

    public static string FirstName() =>
        FirstNames[Index(FirstNames.Length)];

    //- - - - - - - - - - - - - - - - - - -//

    public static string LastName() =>
        LastNames[Index(LastNames.Length)];

    //- - - - - - - - - - - - - - - - - - -//

    public static string Email()
    {
        string firstName = FirstName();
        string lastName = LastName();
        return $"{firstName}.{lastName}{Domain()}";
    }

    //- - - - - - - - - - - - - - - - - - -//

    public static string Phone()
    {
        int areaCode = Number(100, 999);
        int prefix = Number(200, 899);
        int lineNumber = Number(1000000, 9999999);

        return $"({areaCode}) {Prefix()} {lineNumber}";
    }

    //- - - - - - - - - - - - - - - - - - -//

    public static string Username()
    {
        string firstName = FirstName();
        string lastName = LastName();

        // Combine first name and last name without spaces
        string combinedName = firstName.Replace(" ", "") + lastName.Replace(" ", "");

        // Add a random number at the end to ensure uniqueness
        int randomNumber = Number(100, 999);
        string uniquePart = randomNumber.ToString("D3");

        // Combine everything
        string username = combinedName + uniquePart;

        // Ensure the username doesn't exceed 20 characters
        if (username.Length > 20)
            username = username[..20];

        return username.ToLower();
    }

    //------------------------------------//

    // Method to get a random enum value
    public static E Enum<E>() where E : Enum
    {
        var values = System.Enum.GetValues(typeof(E));
        return (E)values.GetValue(new Random().Next(values.Length))!;
    }

    //------------------------------------//

    private static int Index(int max) => new Random().Next(max);

    //- - - - - - - - - - - - - - - - - - -//

    private static string Domain() => Domains[Index(Domains.Length)];

    //- - - - - - - - - - - - - - - - - - -//

    private static string Prefix() => PhonePrefixes[Index(PhonePrefixes.Length)];

    //- - - - - - - - - - - - - - - - - - -//

    private static int Number(int min, int max) => new Random().Next(min, max + 1);

    //------------------------------------//

}//Cls
