namespace ClArch.ValueObjects.Setup;

internal class ValueObjectsSettings
{
    /// <summary>
    /// default=<inheritdoc cref="ValueObjectsDefaultValues.MAX_LENGTH_ADDRESS_LINE"/>
    /// </summary>
    public static int MaxLengthAddressLine { get; set; } = ValueObjectsDefaultValues.MAX_LENGTH_ADDRESS_LINE;

    /// <summary>
    /// default=<inheritdoc cref="ValueObjectsDefaultValues.MAX_LENGTH_DESCRIPTION"/>
    /// </summary>
    public static int MaxLengthDescription { get; set; } = ValueObjectsDefaultValues.MAX_LENGTH_DESCRIPTION;

    /// <summary>
    /// default=<inheritdoc cref="ValueObjectsDefaultValues.MAX_LENGTH_EMAIL"/>
    /// </summary>
    public static int MaxLengthEmail { get; set; } = ValueObjectsDefaultValues.MAX_LENGTH_EMAIL;

    /// <summary>
    /// default=<inheritdoc cref="ValueObjectsDefaultValues.MAX_LENGTH_FILE_NAME"/>
    /// </summary>
    public static int MaxLengthFileName { get; set; } = ValueObjectsDefaultValues.MAX_LENGTH_FILE_NAME;

    /// <summary>
    /// default=<inheritdoc cref="ValueObjectsDefaultValues.MAX_LENGTH_FILE_CONTENT_TYPE"/>
    /// </summary>
    public static int MaxLengthFileContentType { get; set; } = ValueObjectsDefaultValues.MAX_LENGTH_FILE_CONTENT_TYPE;

    /// <summary>
    /// default=<inheritdoc cref="ValueObjectsDefaultValues.MAX_LENGTH_FIRST_NAME"/>
    /// </summary>
    public static int MaxLengthFirstName { get; set; } = ValueObjectsDefaultValues.MAX_LENGTH_FIRST_NAME;

    /// <summary>
    /// default=<inheritdoc cref="ValueObjectsDefaultValues.MAX_LENGTH_RESOURCE_URL"/>
    /// </summary>
    public static int MaxLengthResourceUrl { get; set; } = ValueObjectsDefaultValues.MAX_LENGTH_RESOURCE_URL;

    /// <summary>
    /// default=<inheritdoc cref="ValueObjectsDefaultValues.MAX_LENGTH_LAST_NAME"/>
    /// </summary>
    public static int MaxLengthLastName { get; set; } = ValueObjectsDefaultValues.MAX_LENGTH_LAST_NAME;

    /// <summary>
    /// default=<inheritdoc cref="ValueObjectsDefaultValues.MAX_LENGTH_NAME"/>
    /// </summary>
    public static int MaxLengthName { get; set; } = ValueObjectsDefaultValues.MAX_LENGTH_NAME;

    /// <summary>
    /// default=<inheritdoc cref="ValueObjectsDefaultValues.MAX_LENGTH_PHONE"/>
    /// </summary>
    public static int MaxLengthPhone { get; set; } = ValueObjectsDefaultValues.MAX_LENGTH_PHONE;

    /// <summary>
    /// default=<inheritdoc cref="ValueObjectsDefaultValues.MAX_LENGTH_PWD"/>
    /// </summary>
    public static int MaxLengthPwd { get; set; } = ValueObjectsDefaultValues.MAX_LENGTH_PWD;

    /// <summary>
    /// default=<inheritdoc cref="ValueObjectsDefaultValues.MAX_LENGTH_NOTES_CONTENT"/>
    /// </summary>
    public static int MaxLengthLongNotesContent { get; set; } = ValueObjectsDefaultValues.MAX_LENGTH_NOTES_CONTENT;

    /// <summary>
    /// default=<inheritdoc cref="ValueObjectsDefaultValues.MAX_LENGTH_SHORT_NOTES_CONTENT"/>
    /// </summary>
    public static int MaxLengthShortNotesContent { get; set; } = ValueObjectsDefaultValues.MAX_LENGTH_SHORT_NOTES_CONTENT;

    /// <summary>
    /// default=<inheritdoc cref="ValueObjectsDefaultValues.MAX_LENGTH_MEDIUM_NOTES_CONTENT"/>
    /// </summary>
    public static int MaxLengthMediumNotesContent { get; set; } = ValueObjectsDefaultValues.MAX_LENGTH_MEDIUM_NOTES_CONTENT;


    /// <summary>
    /// default=<inheritdoc cref="ValueObjectsDefaultValues.MAX_LENGTH_USERNAME"/>
    /// </summary>
    public static int MaxLengthUsername { get; set; } = ValueObjectsDefaultValues.MAX_LENGTH_USERNAME;


    //------------------------------------//


    internal static void Setup(ValueObjectsSetupOptions? options)
    {
        if (options == null)
            return;

        MaxLengthAddressLine = options.MaxLengthAddressLine;
        MaxLengthDescription = options.MaxLengthDescription;
        MaxLengthEmail = options.MaxLengthEmail;
        MaxLengthFirstName = options.MaxLengthFirstName;
        MaxLengthResourceUrl = options.MaxLengthResourceUrl;
        MaxLengthLastName = options.MaxLengthLastName;
        MaxLengthName = options.MaxLengthName;
        MaxLengthPhone = options.MaxLengthPhone;
        MaxLengthPwd = options.MaxLengthPwd;
        MaxLengthLongNotesContent = options.MaxLengthNotesContent;
        MaxLengthUsername = options.MaxLengthUsername;
        MaxLengthShortNotesContent = options.MaxLengthShortNotesContent;
        MaxLengthMediumNotesContent = options.MaxLengthMediumNotesContent;
        MaxLengthFileContentType = options.MaxLengthFileContentType;
        MaxLengthFileName = options.MaxLengthFileName;

    }

    //------------------------------------//

}//Cls
