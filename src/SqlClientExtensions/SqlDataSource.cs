using System.Data.Common;

namespace Microsoft.Data.SqlClient;

/// <inheritdoc />
public abstract class SqlDataSource : DbDataSource
{
    /// <inheritdoc />
    public override string ConnectionString { get; }

    /// <inheritdoc />
    protected override DbConnection CreateDbConnection()
        => CreateConnection();

    /// <inheritdoc />
    protected override DbCommand CreateDbCommand(string? commandText = null)
        => CreateCommand();

    /// <summary>
    /// Contains the connection string returned to the user from <see cref="SqlConnection.ConnectionString"/>
    /// after the connection has been opened. Does not contain the password unless Persist Security Info=true.
    /// </summary>
    public SqlConnectionStringBuilder Settings { get; }

    /// <summary>
    /// Returns a new, opened connection from this data source.
    /// </summary>
    public new SqlConnection OpenConnection()
    {
        var connection = CreateConnection();

        try
        {
            connection.Open();
            return connection;
        }
        catch
        {
            connection.Dispose();
            throw;
        }
    }

    public SqlDataSource(
    SqlConnectionStringBuilder settings)
    {
        Settings = settings;
        ConnectionString = settings.ConnectionString;
    }

    /// <summary>
    /// Returns a new, opened connection from this data source.
    /// </summary>
    /// <param name="cancellationToken">
    /// An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/>.
    /// </param>
    public new async ValueTask<SqlConnection> OpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = CreateConnection();

        try
        {
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            return connection;
        }
        catch
        {
            await connection.DisposeAsync().ConfigureAwait(false);
            throw;
        }
    }

    /// <summary>
    /// Returns a new, unopened connection from this data source.
    /// </summary>
    public new SqlConnection CreateConnection()
    {
        return new SqlConnection(ConnectionString);
    }

    /// <summary>
    /// Creates a command ready for use against this <see cref="SqlDataSource" />.
    /// </summary>
    /// <param name="commandText">An optional SQL text for the command.</param>
    public new SqlCommand CreateCommand(string? commandText = null)
    {
        var command = CreateConnection().CreateCommand();
        if (commandText != null)
        { 
            command.CommandText = commandText;
        }
        return command;
    }
    
    /// <summary>
    /// Creates a new <see cref="SqlDataSource" /> for the given <paramref name="connectionString" />.
    /// </summary>
    public static SqlDataSource Create(string connectionString)
        => new SqlDataSourceBuilder(connectionString).Build();

    /// <summary>
    /// Creates a new <see cref="SqlDataSource" /> for the given <paramref name="connectionStringBuilder" />.
    /// </summary>
    public static SqlDataSource Create(SqlConnectionStringBuilder connectionStringBuilder)
        => Create(connectionStringBuilder.ToString());
}