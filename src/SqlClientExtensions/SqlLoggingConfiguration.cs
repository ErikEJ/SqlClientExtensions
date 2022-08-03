using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Microsoft.Data.SqlClient;

/// <summary>
/// Configures SQL logging
/// </summary>
public class SqlLoggingConfiguration
{
    private readonly SqlClientListener? sqlClientListener;
    internal static readonly SqlLoggingConfiguration NullConfiguration
        = new(NullLoggerFactory.Instance, isParameterLoggingEnabled: false);

    internal SqlLoggingConfiguration(ILoggerFactory loggerFactory, bool isParameterLoggingEnabled)
    {
        if (loggerFactory != NullLoggerFactory.Instance)
        {
            sqlClientListener = new SqlClientListener(loggerFactory);
        }
        IsParameterLoggingEnabled = isParameterLoggingEnabled;
    }

    /// <summary>
    /// Determines whether parameter contents will be logged alongside SQL statements - this may reveal sensitive information.
    /// Defaults to false.
    /// </summary>
    internal bool IsParameterLoggingEnabled { get; }
}