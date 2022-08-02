using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSqlDataSource(builder.Configuration.GetConnectionString("DefaultConnection")!);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/hello", async (SqlConnection connection) =>
{
    await connection.OpenAsync();
    await using var command = new SqlCommand("SELECT TOP 1 SupplierID FROM Suppliers", connection);
    return "Hello World: " + await command.ExecuteScalarAsync();
})
.WithName("Hello")
.WithOpenApi();

app.MapGet("/simpler", async (SqlDataSource dataSource) =>
{
    await using var command = dataSource.CreateCommand("SELECT TOP 1 SupplierID FROM Suppliers");
    return "Hello World: " + await command.ExecuteScalarAsync();
})
.WithName("Simpler")
.WithOpenApi();

app.Run();
