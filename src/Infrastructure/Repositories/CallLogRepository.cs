using Application.Interfaces;
using Dapper;
using Domain.Entities;
using Domain.ValueObjects;
using Infrastructure.Persistence;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories;

/// <summary>
/// Repository implementation for CallLog using Dapper and SQL Server
/// </summary>
public class CallLogRepository : ICallLogRepository
{
    private readonly DatabaseConnectionFactory _connectionFactory;
    private readonly ILogger<CallLogRepository> _logger;

    public CallLogRepository(
        DatabaseConnectionFactory connectionFactory,
        ILogger<CallLogRepository> logger)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<CallLog> CreateAsync(CallLog callLog, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            INSERT INTO CallLogs (Id, CallSid, CallerNumber, MenuPath, DigitPressed, CurrentMenuId, Timestamp)
            VALUES (@Id, @CallSid, @CallerNumber, @MenuPath, @DigitPressed, @CurrentMenuId, @Timestamp)";

        try
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(sql, new
            {
                Id = callLog.Id,
                CallSid = callLog.CallSid,
                CallerNumber = callLog.CallerNumber.Value,
                MenuPath = callLog.MenuPath,
                DigitPressed = callLog.DigitPressed,
                CurrentMenuId = callLog.CurrentMenuId,
                Timestamp = callLog.Timestamp
            });

            _logger.LogInformation("Created call log for CallSid {CallSid}", callLog.CallSid);
            return callLog;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating call log for CallSid {CallSid}", callLog.CallSid);
            throw;
        }
    }

    public async Task<IEnumerable<CallLog>> GetAllAsync(
        int skip = 0,
        int take = 100,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT Id, CallSid, CallerNumber, MenuPath, DigitPressed, CurrentMenuId, Timestamp
            FROM CallLogs
            ORDER BY Timestamp DESC
            OFFSET @Skip ROWS
            FETCH NEXT @Take ROWS ONLY";

        try
        {
            using var connection = _connectionFactory.CreateConnection();
            var results = await connection.QueryAsync<CallLogDto>(sql, new { Skip = skip, Take = take });

            return results.Select(MapToEntity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving call logs");
            throw;
        }
    }

    public async Task<IEnumerable<CallLog>> GetByCallSidAsync(
        string callSid,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT Id, CallSid, CallerNumber, MenuPath, DigitPressed, CurrentMenuId, Timestamp
            FROM CallLogs
            WHERE CallSid = @CallSid
            ORDER BY Timestamp ASC";

        try
        {
            using var connection = _connectionFactory.CreateConnection();
            var results = await connection.QueryAsync<CallLogDto>(sql, new { CallSid = callSid });

            return results.Select(MapToEntity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving call logs for CallSid {CallSid}", callSid);
            throw;
        }
    }

    public async Task<IEnumerable<CallLog>> GetByPhoneNumberAsync(
        string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT Id, CallSid, CallerNumber, MenuPath, DigitPressed, CurrentMenuId, Timestamp
            FROM CallLogs
            WHERE CallerNumber = @PhoneNumber
            ORDER BY Timestamp DESC";

        try
        {
            using var connection = _connectionFactory.CreateConnection();
            var results = await connection.QueryAsync<CallLogDto>(sql, new { PhoneNumber = phoneNumber });

            return results.Select(MapToEntity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving call logs for phone {PhoneNumber}", phoneNumber);
            throw;
        }
    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT COUNT(*) FROM CallLogs";

        try
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(sql);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting call log count");
            throw;
        }
    }

    /// <summary>
    /// Maps database DTO to domain entity
    /// </summary>
    private static CallLog MapToEntity(CallLogDto dto)
    {
        var phoneNumber = new PhoneNumber(dto.CallerNumber);
        var callLog = new CallLog(dto.CallSid, phoneNumber, dto.MenuPath, dto.DigitPressed, dto.CurrentMenuId);

        // Use reflection to set the Id and Timestamp (normally not recommended, but for ORMs it's acceptable)
        var idProperty = typeof(CallLog).GetProperty(nameof(CallLog.Id));
        idProperty?.SetValue(callLog, dto.Id);

        var timestampProperty = typeof(CallLog).GetProperty(nameof(CallLog.Timestamp));
        timestampProperty?.SetValue(callLog, dto.Timestamp);

        return callLog;
    }

    /// <summary>
    /// DTO for database mapping
    /// </summary>
    private class CallLogDto
    {
        public Guid Id { get; set; }
        public string CallSid { get; set; } = string.Empty;
        public string CallerNumber { get; set; } = string.Empty;
        public string MenuPath { get; set; } = string.Empty;
        public string DigitPressed { get; set; } = string.Empty;
        public string CurrentMenuId { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
