// IDbConnectionFactory.cs
using System.Data.Common;
internal interface IDbConnectionFactory<TConnection> where TConnection : DbConnection
{
    TConnection CreateConnection();
}
