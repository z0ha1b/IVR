using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Infrastructure.Persistence;

/// <summary>
/// Factory for creating database connections
/// Implements IDisposable for proper connection management
/// </summary>
public class DatabaseConnectionFactory : IDisposable
{
    private readonly string _connectionString;
    private IDbConnection? _connection;

    public DatabaseConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Database connection string not found");
    }

    /// <summary>
    /// Creates and opens a new database connection
    /// </summary>
    public IDbConnection CreateConnection()
    {
        _connection = new SqlConnection(_connectionString);
        _connection.Open();
        return _connection;
    }

    /// <summary>
    /// Gets an existing connection or creates a new one
    /// </summary>
    public IDbConnection GetConnection()
    {
        if (_connection == null || _connection.State != ConnectionState.Open)
        {
            return CreateConnection();
        }
        return _connection;
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }
}
