using ID.Jwt.KeyBuilder.Tests;
using ID.Jwt.KeyBuilder.Tools;

namespace ID.Jwt.KeyBuilder;

/// <summary>
/// Test runner and validator for the JWT Key Builder
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("JWT Key Builder - Test & Validation Tool");
        Console.WriteLine("This will test and validate the BouncyCastle implementation");
        Console.WriteLine();

        if (args.Length > 0)
        {
            HandleCommandLineArgs(args);
            return;
        }

        // Interactive menu
        ShowMenu();
    }

    private static void ShowMenu()
    {
        while (true)
        {
            Console.WriteLine("Select an option:");
            Console.WriteLine("1. Run validation tests (verify implementation works)");
            Console.WriteLine("2. Validate existing key files");
            Console.WriteLine("3. Generate and validate new keys");
            Console.WriteLine("4. Interactive key validator");
            Console.WriteLine("5. Exit");
            Console.Write("\nEnter choice (1-5): ");

            var choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        Console.WriteLine();
                        JwtKeyBuilderValidationTests.RunAllTests();
                        break;
                    case "2":
                        ValidateSpecificKeys();
                        break;
                    case "3":
                        GenerateAndValidate();
                        break;
                    case "4":
                        JwtKeyValidator.RunInteractiveValidator();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please select 1-5.");
                        continue;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }
    }

    private static void ValidateSpecificKeys()
    {
        Console.WriteLine();
        Console.Write("Enter private key file path: ");
        var privatePath = Console.ReadLine();

        Console.Write("Enter public key file path: ");
        var publicPath = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(privatePath) || string.IsNullOrWhiteSpace(publicPath))
        {
            Console.WriteLine("❌ Please provide both file paths");
            return;
        }

        Console.WriteLine();
        JwtKeyValidator.ValidateKeyPair(privatePath, publicPath);
    }

    private static void GenerateAndValidate()
    {
        Console.WriteLine();
        Console.Write("Enter directory to save keys: ");
        var directory = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(directory))
        {
            directory = Path.Combine(Path.GetTempPath(), $"jwt-test-{DateTime.Now:yyyyMMdd-HHmmss}");
            Console.WriteLine($"Using default directory: {directory}");
        }

        try
        {
            Console.WriteLine("\n🔧 Generating 2048-bit keys...");
            var keyData = JwtPemBuilder.GenerateJwtPemKeys(directory);

            Console.WriteLine("✅ Keys generated successfully");
            Console.WriteLine();

            JwtKeyValidator.ValidateKeyPair(keyData.PrivatePath, keyData.PublicPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to generate keys: {ex.Message}");
        }
    }

    private static void HandleCommandLineArgs(string[] args)
    {
        switch (args[0].ToLower())
        {
            case "--help":
            case "-h":
                ShowHelp();
                break;
            case "--test":
                JwtKeyBuilderValidationTests.RunAllTests();
                break;
            case "--validate":
                if (args.Length >= 3)
                {
                    JwtKeyValidator.ValidateKeyPair(args[1], args[2]);
                }
                else
                {
                    Console.WriteLine("Usage: --validate <private-key-path> <public-key-path>");
                }
                break;
            case "--generate":
                var directory = args.Length > 1 ? args[1] : Path.GetTempPath();
                var keyData = JwtPemBuilder.GenerateJwtPemKeys(directory);
                Console.WriteLine($"Keys generated:\nPrivate: {keyData.PrivatePath}\nPublic: {keyData.PublicPath}");
                JwtKeyValidator.ValidateKeyPair(keyData.PrivatePath, keyData.PublicPath);
                break;
            default:
                Console.WriteLine($"Unknown option: {args[0]}");
                ShowHelp();
                break;
        }
    }

    private static void ShowHelp()
    {
        Console.WriteLine("Usage: dotnet run [options]");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  --help, -h                          Show this help message");
        Console.WriteLine("  --test                              Run validation tests");
        Console.WriteLine("  --validate <private> <public>      Validate specific key files");
        Console.WriteLine("  --generate [directory]              Generate and validate keys");
        Console.WriteLine();
        Console.WriteLine("Interactive mode (no arguments):");
        Console.WriteLine("  Provides a menu to choose validation options");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  dotnet run --test");
        Console.WriteLine("  dotnet run --validate private.pem public.pem");
        Console.WriteLine("  dotnet run --generate C:\\temp\\jwt-keys");
    }
}
