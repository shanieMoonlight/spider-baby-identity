namespace ConsoleHelpers;
using StringHelpers;

/// <summary>
/// A helper class for console-based interactions, providing methods for user input and confirmation prompts.
/// </summary>
public class MyConsole
{
    /// <summary>
    /// Repeatedly executes the provided action until the user decides to quit.
    /// </summary>
    /// <param name="act">The action to execute.</param>
    public static void CheckQuit(Action act)
    {
        var quit = false;
        while (!quit)
        {
            act();
            quit = CheckQuit();
            Console.WriteLine($"quit: {quit} ! {!quit}");
        }
    }

    //-----------------------------//

    /// <summary>
    /// Prompts the user with a "Quit Y/N" question and returns true if the user chooses "Y".
    /// </summary>
    /// <returns>True if the user chooses "Y", otherwise false.</returns>
    public static bool CheckQuit() => 
        CheckYesNo("Quit Y/N ??");

    //-----------------------------//

    /// <summary>
    /// Prompts the user with a yes/no question and returns true if the user chooses "Y".
    /// </summary>
    /// <param name="question">The question to display to the user. Without the Y?N part, this will be added for you</param>
    /// <returns>True if the user chooses "Y", otherwise false.</returns>
    public static bool CheckYesNo(string question)
    {
        Console.WriteLine($"{question} Y/N ?? (Hit enter for NO)");
        var YesNo = Console.ReadLine();
        var isYes = IsYesResponse(YesNo);
        Console.WriteLine($"YesNo: '{YesNo}' - ({(isYes ? "YES" : "NO")})");
        return isYes;
    }

    //-----------------------------//

    /// <summary>
    /// Prompts the user for input and repeats the prompt until a valid non-whitespace input is provided.
    /// </summary>
    /// <param name="query">The prompt to display to the user.</param>
    /// <param name="errorMsg">Optional error message to display for invalid input.</param>
    /// <returns>The valid user input.</returns>
    public static string GetInputOrRepeat(string query, string? errorMsg = null)
    {
        Console.WriteLine(query);

        string? userInput = Console.ReadLine();
        while (userInput.IsNullOrWhiteSpace())
        {
            Console.WriteLine(errorMsg ?? $"Invalid Input. {query}");
            userInput = Console.ReadLine();
        }

        return userInput!;
    }

    //-----------------------------//

    /// <summary>
    /// Prompts the user for information with an option to use a default value if no input is provided.
    /// </summary>
    /// <param name="infoName">The name of the information being requested.</param>
    /// <param name="defaultVal">The default value to use if no input is provided.</param>
    /// <returns>The user input or the default value.</returns>
    public static string GetInfo(string infoName, string defaultVal)
    {
        Console.WriteLine($"Enter {infoName}. Just hit enter to use default: \"{defaultVal}\"");
        string? info = Console.ReadLine();
        if (info.IsNullOrWhiteSpace())
            info = defaultVal;
        Console.WriteLine($"You selected: {info}");

        return info!;
    }

    //-----------------------------//

    /// <summary>
    /// Prompts the user for input and allows skipping by pressing enter.
    /// </summary>
    /// <param name="infoName">The name of the information being requested.</param>
    /// <returns>The user input or null if skipped.</returns>
    public static string? GetInputOrNull(string infoName)
    {
        Console.WriteLine($"Enter {infoName}. Just hit enter to skip");
        return Console.ReadLine();
    }

    //-----------------------------//


    private static bool IsYesResponse(string? yesNo) => 
        string.Equals(yesNo?.Trim(), "y", StringComparison.OrdinalIgnoreCase);

}//Cls
