using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Diagnostics.Tracing;

// https://khalidabuhakmeh.com/logging-trace-output-using-ilogger-in-dotnet-applications
internal class SqlClientListener : EventListener
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly bool _verboseLogging;
    private readonly ConcurrentDictionary<string, ILogger> _loggers = new();

    public SqlClientListener(ILoggerFactory loggerFactory, bool enableVerbose)
    {
        _loggerFactory = loggerFactory;
        _verboseLogging = enableVerbose;
    }

    // https://docs.microsoft.com/en-us/sql/connect/ado-net/enable-eventsource-tracing?view=sql-server-ver16
    protected override void OnEventSourceCreated(EventSource eventSource)
    {
        // Only enable events from SqlClientEventSource.
        if (eventSource.Name.Equals("Microsoft.Data.SqlClient.EventSource"))
        {
            var keyWords = _verboseLogging ? EventKeywords.All : (EventKeywords)2;
            // Use EventKeyWord 2 to capture basic application flow events.
            // See the above table for all available keywords.
            EnableEvents(eventSource, EventLevel.Informational, keyWords);
        }
    }

    protected override void OnEventWritten(EventWrittenEventArgs eventData)
    {
        var logger = _loggers.GetOrAdd(
            "Microsoft.Data.SqlClient",
            static (s, factory) => factory.CreateLogger(s),
            _loggerFactory);

        logger.Log(MapLevel(eventData.Level), eventData.Payload?.FirstOrDefault()?.ToString() ?? eventData.Message);
    }

    private LogLevel MapLevel(EventLevel eventLevel) => eventLevel switch
    {
        EventLevel.Verbose => LogLevel.Debug,
        EventLevel.Informational => LogLevel.Information,
        EventLevel.Critical => LogLevel.Critical,
        EventLevel.Error => LogLevel.Error,
        EventLevel.Warning => LogLevel.Warning,
        _ => LogLevel.Trace
    };
}