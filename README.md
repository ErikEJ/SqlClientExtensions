[`Microsoft.Data.SqlClient`](https://github.com/dotnet/SqlClient) is the open source .NET data provider for Microsoft SQL Server. It allows you to connect and interact with SQL Server and Azure SQL Database using .NET.

This package helps set up SqlClient in applications using dependency injection, notably ASP.NET and Worker Service applications. It allows easy configuration of your database connections and registers the appropriate services in your DI container. 

For example, if using the ASP.NET minimal web API, simply use the following to register `Microsoft.Data.SqlClient`:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSqlDataSource("Server=.\\SQLEXPRESS;Database=Northwind;Integrated Security=true;Trust Server Certificate=true");
```

This registers a transient [`SqlConnection`](https://docs.microsoft.com/dotnet/api/microsoft.data.sqlclient.sqlconnection) which can get injected into your controllers:

```csharp
app.MapGet("/", async (SqlConnection connection) =>
{
    await connection.OpenAsync();
    await using var command = new SqlCommand("SELECT TOP 1 SupplierID FROM Suppliers", connection);
    return "Hello World: " + await command.ExecuteScalarAsync();
});
```

But wait! If all you want is to execute some simple SQL, just use the singleton `SqlDataSource` to execute a command directly:

```csharp
app.MapGet("/", async (SqlDataSource dataSource) =>
{
    await using var command = dataSource.CreateCommand("SELECT TOP 1 SupplierID FROM Suppliers");
    return "Hello World: " + await command.ExecuteScalarAsync();
});
```

`SqlDataSource` can also come in handy when you need more than one connection:

```csharp
app.MapGet("/", async (SqlDataSource dataSource) =>
{
    await using var connection1 = await dataSource.OpenConnectionAsync();
    await using var connection2 = await dataSource.OpenConnectionAsync();
    // Use the two connections...
});
```

The AddSqlDataSource method also enables logging of `Microsoft.Data.SqlClient` activity in your ASP.NET Core app.

By default informational messages are logged, this can be configured via logging configuration:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.Data.SqlClient": "Warning"
    }
  }
}
```

You can also disable SqlClient logging completely like this:

```csharp
   builder.Services.AddSqlDataSource("Server=.\\SQLEXPRESS;Database=Northwind;Integrated Security=true;Trust Server Certificate=true", setupAction =>
   {
       setupAction.UseLoggerFactory(null);
   });
```

And you can turn on full logging like this:

```csharp
   builder.Services.AddSqlDataSource("Server=.\\SQLEXPRESS;Database=Northwind;Integrated Security=true;Trust Server Certificate=true", setupAction =>
   {
       setupAction.EnableVerboseLogging();
   });
```

For more information, [see the SqlClient documentation](https://docs.microsoft.com/sql/connect/ado-net/introduction-microsoft-data-sqlclient-namespace).