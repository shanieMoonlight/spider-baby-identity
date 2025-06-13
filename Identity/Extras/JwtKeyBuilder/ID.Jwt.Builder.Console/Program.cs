
using ConsoleHelpers;
using ID.Jwt.KeyBuilder;



string NL = Environment.NewLine;

var DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop); ;
var DefaultDestinationDir = Path.Join(DesktopPath, "MyJwtKeys");
var destinationDir = DefaultDestinationDir;

//-----------------------------//

Console.WriteLine();
Console.WriteLine("#####################################");
Console.WriteLine();
Console.WriteLine("🔑 Hello, I'm JWT Key Builder! 🔑");
Console.WriteLine();
Console.WriteLine("🔑 I build Security Keys for use with in Jwt Creation! 🔑");
Console.WriteLine();
Console.WriteLine("#####################################");
Console.WriteLine();

//-----------------------------//

MyConsole.CheckQuit(() =>
{

    var destinationDir = MyConsole.GetInfo($"Destination Directory.", DefaultDestinationDir);
    
    var keys = JwtPemBuilder.GenerateJwtPemKeys(destinationDir);

    PemToXml.Save(keys.PrivatePath, "private.xml");
    PemToXml.Save(keys.PublicPath, "public.xml");

});

//-----------------------------//
