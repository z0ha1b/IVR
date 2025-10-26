using Domain.Entities;

namespace Application.Interfaces;

/// <summary>
/// Repository interface for persisting and retrieving call logs
/// </summary>
public interface ICallLogRepository
{
    /// <summary>
    /// Creates a new call log entry
    /// </summary>
    Task<CallLog> CreateAsync(CallLog callLog, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all call logs with optional filtering and pagination
    /// </summary>
    Task<IEnumerable<CallLog>> GetAllAsync(
        int skip = 0,
        int take = 100,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves call logs for a specific call session
    /// </summary>
    Task<IEnumerable<CallLog>> GetByCallSidAsync(
        string callSid,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves call logs for a specific phone number
    /// </summary>
    Task<IEnumerable<CallLog>> GetByPhoneNumberAsync(
        string phoneNumber,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of call logs
    /// </summary>
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);
}
