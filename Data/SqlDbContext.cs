using Microsoft.Data.SqlClient;

namespace WeatherCacheApi.Data;

public class SqlDbContext : IAsyncDisposable
{
    private readonly string _connectionString;
    private readonly ILogger<SqlDbContext> _logger;

    private SqlConnection? _connection;
    private SqlTransaction? _transaction;

    public SqlDbContext(IConfiguration configuration, ILogger<SqlDbContext> logger)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");
        _logger = logger;
    }

    // Opens a connection 
    private async Task<SqlConnection> GetOpenConnectionAsync()
    {
        if (_connection is null)
        {
            _connection = new SqlConnection(_connectionString);
            await _connection.OpenAsync();
            _logger.LogDebug("SQL connection opened.");
        }
        return _connection;
    }

    // Begins a transaction on the current/ newly opened connection
    public async Task BeginTransactionAsync()
    {
        var conn = await GetOpenConnectionAsync();

        if (_transaction is not null)
            throw new InvalidOperationException("A transaction is already in progress.");

        _transaction = conn.BeginTransaction();
        _logger.LogDebug("Transaction begun.");
    }

    // Commits the active transaction
    public async Task CommitAsync()
    {
        if (_transaction is null)
            throw new InvalidOperationException("No active transaction to commit.");

        await _transaction.CommitAsync();
        await _transaction.DisposeAsync();
        _transaction = null;
        _logger.LogDebug("Transaction committed.");
    }

    // Rolls back the active transaction
    public async Task RollbackAsync()
    {
        if (_transaction is null)
            throw new InvalidOperationException("No active transaction to roll back.");

        await _transaction.RollbackAsync();
        await _transaction.DisposeAsync();
        _transaction = null;
        _logger.LogDebug("Transaction rolled back.");
    }



    // commits on success, rolls back on any exception, then re-throws
    public async Task ExecuteInTransactionAsync(Func<Task> action)
    {
        await BeginTransactionAsync();
        try
        {
            await action();
            await CommitAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction rolled back due to exception.");
            await RollbackAsync();
            throw;
        }
    }

    private async Task<SqlCommand> CreateCommandAsync(string sql, IEnumerable<SqlParameter>? parameters)
    {
        var conn = await GetOpenConnectionAsync();
        var cmd = new SqlCommand(sql, conn, _transaction);

        if (parameters is not null)
            cmd.Parameters.AddRange(parameters.ToArray());

        return cmd;
    }

    // -----------------------------------------------------------

    // Executes a select and maps every row
    public async Task<List<T>> QueryAsync<T>(
        string sql,
        Func<SqlDataReader, T> map,
        IEnumerable<SqlParameter>? parameters = null)
    {
        await using var cmd = await CreateCommandAsync(sql, parameters);
        await using var reader = await cmd.ExecuteReaderAsync();

        var results = new List<T>();
        while (await reader.ReadAsync())
            results.Add(map(reader));

        return results;
    }

    // Executes a select and maps the first row, or returns null
    public async Task<T?> QuerySingleOrDefaultAsync<T>(
        string sql,
        Func<SqlDataReader, T> map,
        IEnumerable<SqlParameter>? parameters = null) where T : class
    {
        await using var cmd = await CreateCommandAsync(sql, parameters);
        await using var reader = await cmd.ExecuteReaderAsync();

        return await reader.ReadAsync() ? map(reader) : null;
    }

    // Executes INSERT/ UPDATE/ DELETE
    public async Task<int> ExecuteAsync(
        string sql,
        IEnumerable<SqlParameter>? parameters = null)
    {
        await using var cmd = await CreateCommandAsync(sql, parameters);
        return await cmd.ExecuteNonQueryAsync();
    }

    // Executes a statement and returns the first column of the first row
    public async Task<T?> ExecuteScalarAsync<T>(
        string sql,
        IEnumerable<SqlParameter>? parameters = null)
    {
        await using var cmd = await CreateCommandAsync(sql, parameters);
        var result = await cmd.ExecuteScalarAsync();
        return result is null or DBNull ? default : (T)result;
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction is not null)
        {
            _logger.LogWarning("Disposing SqlDbContext with an uncommitted transaction — rolling back.");
            await RollbackAsync();
        }

        if (_connection is not null)
        {
            await _connection.DisposeAsync();
            _connection = null;
            _logger.LogDebug("SQL connection closed.");
        }
    }
}
