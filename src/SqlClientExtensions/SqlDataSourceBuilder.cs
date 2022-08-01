using Microsoft.Extensions.Logging;

namespace Microsoft.Data.SqlClient
{
    /// <summary>
    /// Provides a simple API for configuring and creating an <see cref="SqlDataSource" />, from which database connections can be obtained.
    /// </summary>
    public class SqlDataSourceBuilder
    {
        ILoggerFactory? _loggerFactory;
        bool _sensitiveDataLoggingEnabled;

        /// <summary>
        /// A connection string builder that can be used to configured the connection string on the builder.
        /// </summary>
        public SqlConnectionStringBuilder ConnectionStringBuilder { get; }

        /// <summary>
        /// Returns the connection string, as currently configured on the builder.
        /// </summary>
        public string ConnectionString => ConnectionStringBuilder.ToString();

        /// <summary>
        /// Constructs a new <see cref="SqlDataSourceBuilder" />, optionally starting out from the given <paramref name="connectionString"/>.
        /// </summary>
        public SqlDataSourceBuilder(string? connectionString = null)
            => ConnectionStringBuilder = new SqlConnectionStringBuilder(connectionString);

        /// <summary>
        /// Sets the <see cref="ILoggerFactory" /> that will be used for logging.
        /// </summary>
        /// <param name="loggerFactory">The logger factory to be used.</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public SqlDataSourceBuilder UseLoggerFactory(ILoggerFactory? loggerFactory)
        {
            _loggerFactory = loggerFactory;
            return this;
        }

        /// <summary>
        /// Enables parameters to be included in logging. This includes potentially sensitive information from data sent to SQL Server.
        /// You should only enable this flag in development, or if you have the appropriate security measures in place based on the
        /// sensitivity of this data.
        /// </summary>
        /// <param name="parameterLoggingEnabled">If <see langword="true" />, then sensitive data is logged.</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public SqlDataSourceBuilder EnableParameterLogging(bool parameterLoggingEnabled = true)
        {
            _sensitiveDataLoggingEnabled = parameterLoggingEnabled;
            return this;
        }

        /// <summary>
        /// Builds and returns an <see cref="NpgsqlDataSource" /> which is ready for use.
        /// </summary>
        public SqlDataSource Build()
        {
            return new BasicSqlDataSource(ConnectionStringBuilder);
        }
    }
}