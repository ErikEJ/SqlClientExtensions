namespace Microsoft.Data.SqlClient;

/// <inheritdoc />
public class BasicSqlDataSource : SqlDataSource
{
    public BasicSqlDataSource(SqlConnectionStringBuilder settings) : base(settings)
    {
    }
}
