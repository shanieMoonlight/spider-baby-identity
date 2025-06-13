// IdGlobalConstants.Db.cs
namespace ID.GlobalSettings.Constants;

internal partial class IdGlobalConstants
{
    internal static partial class Db
    {
        /// <summary>
        ///  Used to differentiate between other db tables
        /// </summary>
        internal const string SCHEMA = "MyId";
        public const string MIGRATIONS_HISTORY_TABLE = "__EFMigrationsHistory";
    }
}
