using ID.GlobalSettings.Setup.Defaults;
using StringHelpers;


namespace ID.GlobalSettings.Setup.Options;


public class IdGlobalOptions
{
    /// <summary>
    /// Name of app that the user is subscribing to. Will be used in emails, tests etc. 
    /// <para></para>
    /// Required
    /// </summary>
    public required string ApplicationName { get; set; } = string.Empty;


    /// <summary>
    /// The url that locates the Customer accounts section of your site.
    /// Will be used for links in emails (ForgotPwd, Registration etc)
    /// </summary>
    public required string MntcAccountsUrl { get; set; } = string.Empty;


    private int _mntcMinTeamPosition = IdGlobalDefaultValues.MIN_TEAM_POSITION;
    /// <summary>
    /// Lowest team position available. Default = <inheritdoc cref="IdGlobalDefaultValues.MIN_TEAM_POSITION"/> 
    /// </summary>
    public required int MntcTeamMinPosition
    {
        get => _mntcMinTeamPosition;
        set => _mntcMinTeamPosition = value < 1
            ? IdGlobalDefaultValues.MIN_TEAM_POSITION
            : value;
    }


    private int _mntcMaxTeamPosition = IdGlobalDefaultValues.MAX_TEAM_POSITION;
    /// <summary>
    /// Highest team position available.
    /// <para></para>
    /// Default = <inheritdoc cref="IdGlobalDefaultValues.MAX_TEAM_POSITION"/> 
    /// </summary>
    public required int MntcTeamMaxPosition
    {
        get => _mntcMaxTeamPosition;
        set => _mntcMaxTeamPosition = value < 1
            ? IdGlobalDefaultValues.MAX_TEAM_POSITION
            : value;
    }



    private int _superTeamMinPosition = IdGlobalDefaultValues.MIN_TEAM_POSITION;
    /// <summary>
    /// Lowest team position available. Super = <inheritdoc cref="IdGlobalSuperValues.MIN_TEAM_POSITION"/> 
    /// </summary>
    public required int SuperTeamMinPosition
    {
        get => _superTeamMinPosition;
        set => _superTeamMinPosition = value < 1
            ? IdGlobalDefaultValues.MIN_TEAM_POSITION
            : value;
    }


    private int _superTeamMaxPosition = IdGlobalDefaultValues.MAX_TEAM_POSITION;
    /// <summary>
    /// Highest team position available.
    /// <para></para>
    /// Super = <inheritdoc cref="IdGlobalDefaultValues.MAX_TEAM_POSITION"/> 
    /// </summary>
    public required int SuperTeamMaxPosition
    {
        get => _superTeamMaxPosition;
        set => _superTeamMaxPosition = value < 1
            ? IdGlobalDefaultValues.MAX_TEAM_POSITION
            : value;
    }


    private string _claimTypePrefix = IdGlobalDefaultValues.CLAIM_TYPE_PREFIX;
    /// <summary>
    /// Claim Identifier
    /// Default = <inheritdoc cref="IdGlobalDefaultValues.CLAIM_TYPE_PREFIX"/>
    /// </summary>
    public required string ClaimTypePrefix
    {
        get => _claimTypePrefix;
        set => _claimTypePrefix = value.IsNullOrWhiteSpace() 
            ? IdGlobalDefaultValues.CLAIM_TYPE_PREFIX 
            : value;
    }

    /// <summary>
    /// Whether to use the RefreshTokens alongside the JWT Tokens.
    /// This will result in extra Database calls to check the tokens.
    /// You should probably reduce the expiration time of the JWT tokens if you are using this.
    /// <para></para>
    /// Default = <inheritdoc cref="IdGlobalDefaultValues.REFRESH_TOKENS_ENABLED"/>
    /// </summary>
    public required bool JwtRefreshTokensEnabled { get; set; } = IdGlobalDefaultValues.REFRESH_TOKENS_ENABLED;



    //private TimeSpan _refreshTokenTimeSpan = IdGlobalDefaultValues.REFRESH_TOKEN_EXPIRE_TIME_SPAN;
    ///// <summary>
    ///// Whether to use the RefreshTokens alongside the JWT Tokens.
    ///// This will result in extra Database calls to check the tokens.
    ///// You should probably reduce the expiration time of the JWT tokens if you are using this.
    ///// <para></para>
    ///// Default = <inheritdoc cref="IdGlobalDefaultValues.REFRESH_TOKEN_EXPIRE_TIME_SPAN"/>
    ///// </summary>
    //public required TimeSpan RefreshTokenTimeSpan
    //{
    //    get => _refreshTokenTimeSpan;
    //    set => _refreshTokenTimeSpan = value <= TimeSpan.Zero
    //        ? IdGlobalDefaultValues.REFRESH_TOKEN_EXPIRE_TIME_SPAN
    //        : value;
    //}


    private TimeSpan _phoneTokenTimeSpan = IdGlobalDefaultValues.PHONE_TOKEN_EXPIRE_TIME_SPAN;
    /// <summary>
    /// The Phone Confirmation Token expiration time. (Used only with DB Token Provider)
    /// <para></para>
    /// Default = <inheritdoc cref="IdGlobalDefaultValues.PHONE_TOKEN_EXPIRE_TIME_SPAN"/>
    /// </summary>
    public required TimeSpan PhoneTokenTimeSpan
    {
        get => _phoneTokenTimeSpan;
        set => _phoneTokenTimeSpan = value <= TimeSpan.Zero
            ? IdGlobalDefaultValues.PHONE_TOKEN_EXPIRE_TIME_SPAN
            : value;
    }

}//Cls
