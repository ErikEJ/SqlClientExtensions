#if !NET8_0_OR_GREATER

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member (compatibility shim for old TFMs)

// ReSharper disable once CheckNamespace
namespace System.Data.Common;

public abstract class DbDataSource : IDisposable, IAsyncDisposable
{
    public abstract string ConnectionString { get; }

    protected abstract DbConnection CreateDbConnection();

    protected virtual DbConnection OpenDbConnection()
        => throw new NotSupportedException();

    protected virtual ValueTask<DbConnection> OpenDbConnectionAsync(CancellationToken cancellationToken = default)
        => throw new NotSupportedException();

    protected virtual DbCommand CreateDbCommand(string? commandText = null)
        => throw new NotSupportedException();

    public DbConnection CreateConnection()
        => CreateDbConnection();

    public DbConnection OpenConnection()
        => OpenDbConnection();

    public ValueTask<DbConnection> OpenConnectionAsync(CancellationToken cancellationToken = default)
        => OpenDbConnectionAsync(cancellationToken);

    public DbCommand CreateCommand(string? commandText = null)
        => CreateDbCommand();

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);

        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
    }

    protected virtual ValueTask DisposeAsyncCore()
        => default;
}

#endif