namespace ID.Application.Utility;

/// <summary>
/// Centralized logging event IDs for structured logging and monitoring.
/// ID ranges are allocated to prevent conflicts:
/// 1-99: System events
/// 300-399: Domain-specific events (Teams, etc.)
/// 400-499: Infrastructure events (MediatR, etc.)
/// 600-699: Authentication/User events
/// 1000+: CRUD operations
/// </summary>
public class MyIdLoggingEvents
{
    #region System Events (1-99)
    public const int UNEXPECTED = 1;
    public const int STARTUP_ERROR = 2;
    #endregion

    #region Authentication Events (600-699)
    public const int LOGIN_ATTEMPT = 600;
    public const int ALREADY_REGISTERED = 601;
    public const int AZURE = 602;
    public const int NEW_USER_SIGN_UP = 603;
    public const int NEW_COMPANY_SIGN_UP = 604;
    public const int EMAIL = 605;
    public const int EMAIL_CONFIRMATION = 606;
    public const int CANCELLATION = 607;
    public const int WEBSITE_ERROR = 650;
    public const int APP_ERROR = 651;
    #endregion

    #region CRUD Operations (1000+)
    public const int GENERATE_ITEMS = 1000;
    public const int LIST_ITEMS = 1001;
    public const int GET_ITEM = 1002;
    public const int INSERT_ITEM = 1003;
    public const int UPDATE_ITEM = 1004;
    public const int DELETE_ITEM = 1005;
    #endregion
    #region Domain Events (300-349)
    public static class TEAMS
    {
        public const int NOT_FOUND = 300;
    }
    #endregion

    #region Background Jobs (350-399)  
    public static class JOBS
    {
        public const int OUTBOX_PROCESSING = 350;
        public const int OLD_OUTBOX_MSGS_PROCESSING = 351;
        public const int OLD_LOGS_PROCESSING = 352;
        public const int SCHEDULE_EXTENSION = 353;
        public const int PRICING_MGMT = 354;
        public const int DB_MNTC = 355;
    }
    #endregion

    #region Infrastructure Events (400-499)
    public static class MEDIATR
    {
        public const int UNKNOWN = 400;
    }

    public static class DB
    {
        public const int InvalidProperty = 450;
    }
    #endregion

}//Cls