using StronglyTypedAppSettings;

namespace MyIdDemo.Setup.Data;

/// <summary>
/// Class with config data for this app and it's libraries
/// </summary>
public class StartupData(IConfiguration config, IWebHostEnvironment Environment) : AppSettingsAccessor(config)
{

    /// <summary>
    /// Name of this application
    /// </summary>
    public string APP_NAME => "MyId Demo";

    /// <summary>
    /// Root url this application
    /// </summary>
    protected string BASE_URL => "https://stelga.ie";


    /// <summary>
    /// Root url this application, during production
    /// </summary>
    protected string BASE_URL_DEV => "https://localhost:44380";

    /// <summary>
    /// Company Colors. Used in Emails etc.
    /// </summary>
    public string COLOR_HEX_BRAND => "#eeb313";

    /// <summary>
    /// Name of company that this app is for
    /// </summary>
    public string COMPANY_NAME => "Spider Baby";

    /// <summary>
    /// Name to appear in Logging Emails
    /// </summary>
    public string COMPANY_NAME_LOGGING => "Spider Baby Web";

    /// <summary>
    /// Where is the Company Logo stored. USed in emails etc.
    /// </summary>
    //protected string LOGO_FILE_NAME => "logo.png";
    protected string LOGO_FILE_NAME => "stelga_logo.png";



    /// <summary>
    /// Default: "dist"
    /// </summary>
    public static string SPA_DIST_FOLDER { get; set; } = "dist";

    /// <summary>
    /// Default: "ClientApp"
    /// </summary>
    public static string SPA_ROOT { get; set; } = "ClientApp";


    /// <summary>
    /// Full Path to the production distribution directory.
    /// </summary>
    protected string DIST_DIRECTORY_PROD = Path.Combine(Directory.GetCurrentDirectory(), SPA_ROOT, SPA_DIST_FOLDER);


    /// <summary>
    /// Path to the static files for the Single Page Application (SPA).
    /// </summary>
    public string SPA_STATIC_FILES_PATH = Path.Combine(SPA_ROOT, SPA_DIST_FOLDER);


    /// <summary>
    /// Reads the contents of an XML file and returns it as a string.
    /// </summary>
    /// <param name="xmlFilePath">The path to the XML file.</param>
    /// <returns>The contents of the XML file as a string.</returns>
    public static string XmlFileToString(string xmlFilePath)
    {
        using var sr = File.OpenText(xmlFilePath);
        return sr.ReadToEnd();
    }


    /// <summary>
    /// Gets the XML string of the asymmetric public key from the Jwt/key.public.xml file.
    /// </summary>
    /// <returns>The XML string of the public key.</returns>
    public string GetAsymmetricPublicKeyXmlString()
        => XmlFileToString(Path.Join("JwtKeys", "public.xml"));


    /// <summary>
    /// Gets the XML string of the asymmetric private key from the Jwt/key.private.xml file.
    /// </summary>
    /// <returns>The XML string of the private key.</returns>
    public string GetAsymmetricPrivateKeyXmlString()
        => XmlFileToString(Path.Join("JwtKeys", "private.xml"));



    /// <summary>
    /// Gets the URL of the company logo.
    /// </summary>
    /// <returns>A string representing the URL of the company logo.</returns>
    public string GetLogoUrl() =>
        "https://drive.google.com/file/d/1KDED_3o4dMV98rbSThl4TzKiPAu3JEGj/view";



    /// <summary>
    /// Gets the base URL for the application, depending on the environment.
    /// </summary>
    /// <returns>The base URL as a string.</returns>
    public string GetBaseUrl() =>
        Environment.IsDevelopment() ? BASE_URL_DEV : BASE_URL;



    /// <summary>
    /// Gets the production base URL for the application.
    /// </summary>
    /// <returns>The production base URL as a string.</returns>
    public string GetBaseUrlProd() => new(BASE_URL);


    /// <summary>
    /// Gets the base URI for the application, depending on the environment.
    /// </summary>
    /// <returns>The base URI as a <see cref="Uri"/> object.</returns>
    public Uri GetBaseUri() => new(GetBaseUrl());


}//Cls


