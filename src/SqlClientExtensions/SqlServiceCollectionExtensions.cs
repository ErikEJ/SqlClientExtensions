using System.Data.Common;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension method for setting up SqlClient services in an <see cref="IServiceCollection" />.
/// </summary>
public static class SqlServiceCollectionExtensions
{
    /// <summary>
    /// Registers an <see cref="SqlDataSource" /> and an <see cref="SqlConnection" /> in the <see cref="IServiceCollection" />,
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="connectionString">An SQL Server connection string.</param>
    /// <param name="dataSourceBuilderAction">
    /// An action to configure the <see cref="SqlDataSourceBuilder" /> for further customizations of the <see cref="SqlDataSource" />.
    /// </param>
    /// <param name="connectionLifetime">
    /// The lifetime with which to register the <see cref="SqlConnection" /> in the container.
    /// Defaults to <see cref="ServiceLifetime.Scoped" />.
    /// </param>
    /// <param name="dataSourceLifetime">
    /// The lifetime with which to register the <see cref="SqlDataSource" /> service in the container.
    /// Defaults to <see cref="ServiceLifetime.Singleton" />.
    /// </param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddSqlDataSource(
        this IServiceCollection serviceCollection,
        string connectionString,
        Action<SqlDataSourceBuilder> dataSourceBuilderAction,
        ServiceLifetime connectionLifetime = ServiceLifetime.Transient,
        ServiceLifetime dataSourceLifetime = ServiceLifetime.Singleton)
        => AddSqlDataSourceCore(serviceCollection, connectionString, dataSourceBuilderAction, connectionLifetime, dataSourceLifetime);

    /// <summary>
    /// Registers an <see cref="SqlDataSource" /> and an <see cref="SqlConnection" /> in the <see cref="IServiceCollection" />,
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="connectionString">An SQL Server connection string.</param>
    /// <param name="connectionLifetime">
    /// The lifetime with which to register the <see cref="SqlConnection" /> in the container.
    /// Defaults to <see cref="ServiceLifetime.Scoped" />.
    /// </param>
    /// <param name="dataSourceLifetime">
    /// The lifetime with which to register the <see cref="SqlDataSource" /> service in the container.
    /// Defaults to <see cref="ServiceLifetime.Singleton" />.
    /// </param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddSqlDataSource(
        this IServiceCollection serviceCollection,
        string connectionString,
        ServiceLifetime connectionLifetime = ServiceLifetime.Transient,
        ServiceLifetime dataSourceLifetime = ServiceLifetime.Singleton)
        => AddSqlDataSourceCore(
            serviceCollection, connectionString, dataSourceBuilderAction: null, connectionLifetime, dataSourceLifetime);

    static IServiceCollection AddSqlDataSourceCore(
        this IServiceCollection serviceCollection,
        string connectionString,
        Action<SqlDataSourceBuilder>? dataSourceBuilderAction,
        ServiceLifetime connectionLifetime = ServiceLifetime.Transient,
        ServiceLifetime dataSourceLifetime = ServiceLifetime.Singleton)
    {
        serviceCollection.TryAdd(
            new ServiceDescriptor(
                typeof(SqlDataSource),
                sp =>
                {
                    var dataSourceBuilder = new SqlDataSourceBuilder(connectionString);
                    dataSourceBuilder.UseLoggerFactory(sp.GetService<ILoggerFactory>());
                    dataSourceBuilderAction?.Invoke(dataSourceBuilder);
                    return dataSourceBuilder.Build();
                },
                dataSourceLifetime));

        serviceCollection.TryAdd(
            new ServiceDescriptor(
                typeof(SqlConnection),
                sp => sp.GetRequiredService<SqlDataSource>().CreateConnection(),
                connectionLifetime));

        serviceCollection.TryAdd(
            new ServiceDescriptor(
                typeof(DbDataSource),
                sp => sp.GetRequiredService<SqlDataSource>(),
                dataSourceLifetime));

        serviceCollection.TryAdd(
            new ServiceDescriptor(
                typeof(DbConnection),
                sp => sp.GetRequiredService<SqlConnection>(),
                connectionLifetime));

        return serviceCollection;
    }
}