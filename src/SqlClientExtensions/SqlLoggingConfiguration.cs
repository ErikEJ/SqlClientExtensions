using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Microsoft.Data.SqlClient;

/// <summary>
/// Configures SQL logging
/// </summary>
public class SqlLoggingConfiguration
{
    internal static readonly SqlLoggingConfiguration NullConfiguration
        = new(NullLoggerFactory.Instance, false);

    internal SqlLoggingConfiguration(ILoggerFactory loggerFactory, bool enableVerbose)
    {
        if (loggerFactory != NullLoggerFactory.Instance)
        {
            _ = new SqlClientListener(loggerFactory, enableVerbose);
        }
    }
}