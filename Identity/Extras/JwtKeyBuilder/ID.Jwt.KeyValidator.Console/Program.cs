using ID.Jwt.KeyBuilder.Tools;


/// <summary>
/// JWT Key Validator Console Application
/// Validates JWT RSA keys for correctness and compatibility
/// </summary>
Console.WriteLine("🔑 JWT Key Validator");
Console.WriteLine("=" + new string('=', 50));
Console.WriteLine();

if (args.Length > 0)
{
    HandleCommandLineArgs(args);
    return;
}

// Interactive mode
RunInteractiveValidator();


static void HandleCommandLineArgs(string[] args)
{
    switch (args[0].ToLower())
    {
        case "--help":
        case "-h":
            ShowHelp();
            break;
        case "--validate":
            if (args.Length >= 3)
            {
                JwtKeyValidator.ValidateKeyPair(args[1], args[2]);
            }
            else
            {
                Console.WriteLine("❌ Usage: --validate <private-key-path> <public-key-path>");
            }
            break;
        case "--content":
            if (args.Length >= 3)
            {
                JwtKeyValidator.ValidateKeyContent(args[1], args[2]);
            }
            else
            {
                Console.WriteLine("❌ Usage: --content <private-key-content> <public-key-content>");
            }
            break;
        default:
            Console.WriteLine($"❌ Unknown option: {args[0]}");
            ShowHelp();
            break;
    }
}
static void RunInteractiveValidator()
{
    // Use the centralized validator from the main library
    JwtKeyValidator.RunInteractiveValidator();
}
static void ShowHelp()
{
    Console.WriteLine("JWT Key Validator - Command Line Usage");
    Console.WriteLine();
    Console.WriteLine("Usage:");
    Console.WriteLine("  dotnet run [options]");
    Console.WriteLine();
    Console.WriteLine("Options:");
    Console.WriteLine("  --help, -h                          Show this help message");
    Console.WriteLine("  --validate <private> <public>      Validate key files");
    Console.WriteLine("  --content <private> <public>       Validate key content strings");
    Console.WriteLine();
    Console.WriteLine("Interactive mode (no arguments):");
    Console.WriteLine("  Provides a menu to choose validation options");
    Console.WriteLine();
    Console.WriteLine("Examples:");
    Console.WriteLine("  dotnet run --validate private.pem public.pem");
    Console.WriteLine("  dotnet run --help");
    Console.WriteLine();
    Console.WriteLine("Validation tests:");
    Console.WriteLine("  ✅ PEM format validation");
    Console.WriteLine("  ✅ .NET RSA import compatibility");
    Console.WriteLine("  ✅ Key size consistency");
    Console.WriteLine("  ✅ Encryption/decryption test");
    Console.WriteLine("  ✅ Digital signature test");
    Console.WriteLine("  ✅ JWT RS256 compatibility test");
}
