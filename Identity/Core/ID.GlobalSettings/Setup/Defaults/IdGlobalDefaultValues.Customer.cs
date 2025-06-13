namespace ID.GlobalSettings.Setup.Defaults;

#pragma warning disable IDE1006 // Naming Styles
public partial class IdGlobalDefaultValues
{
    internal static class Customer
    {
        //Highest team position, used in  creation and updates (Customer Team - NO Team)
        /// <summary>
        /// 1
        /// </summary>
        internal const int MAX_TEAM_POSITION = 1;


        //Lowest team position, used in  creation and updates (Customer Team - NO Team)
        /// <summary>
        /// 1
        /// </summary>
        internal const int MIN_TEAM_POSITION = 1;

        //Maximum amount of members allowed on a Customer Team
        /// <summary>
        /// 1 (No Customer teams)
        /// </summary>
        internal const int MAX_TEAM_SIZE = 1;
    }


}//Cls
#pragma warning restore IDE1006 // Naming Styles