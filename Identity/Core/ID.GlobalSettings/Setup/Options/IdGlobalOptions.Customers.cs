using ID.GlobalSettings.Setup.Defaults;

namespace ID.GlobalSettings.Setup.Options;


//Not using REQUIRED modifier because the user will be setting these values, not the API project
public class IdGlobalSetupOptions_CUSTOMER
{
    /// <summary>
    /// The url that locates the Customer accounts section of your site.
    /// Will be used for links in emails (ForgotPwd, Registration etc)
    /// </summary>
    public string CustomerAccountsUrl { get; set; } = "";



    private int _maxTeamPosition = IdGlobalDefaultValues.Customer.MAX_TEAM_POSITION;
    /// <summary>
    /// Highest team position available.
    /// <para></para>
    /// Default = <inheritdoc cref="IdGlobalDefaultValues.Customer.MAX_TEAM_POSITION"/> 
    /// </summary>
    public int MaxTeamPosition
    {
        get => _maxTeamPosition;
        set => _maxTeamPosition = value <= 0
            ? IdGlobalDefaultValues.Customer.MAX_TEAM_POSITION
            : value;
    }



    private int _minTeamPosition = IdGlobalDefaultValues.Customer.MIN_TEAM_POSITION;
    /// <summary>
    /// Lowest team position available.
    /// <para></para>
    /// Default = <inheritdoc cref="IdGlobalDefaultValues.Customer.MIN_TEAM_POSITION"/> 
    /// </summary>
    public int MinTeamPosition
    {
        get => _minTeamPosition;
        set => _minTeamPosition = value <= 0
            ? IdGlobalDefaultValues.Customer.MIN_TEAM_POSITION
            : value;
    }



    private int _maxTeamSize = IdGlobalDefaultValues.Customer.MAX_TEAM_SIZE;
    /// <summary>
    /// Maximum amount of members allowed on a Customer Team.
    /// <para></para>
    /// Default = <inheritdoc cref="IdGlobalDefaultValues.Customer.MAX_TEAM_SIZE"/>
    /// </summary>
    public int MaxTeamSize
    {
        get => _maxTeamSize;
        set => _maxTeamSize = value <= 0
            ? IdGlobalDefaultValues.Customer.MAX_TEAM_SIZE
            : value;
    }


}



