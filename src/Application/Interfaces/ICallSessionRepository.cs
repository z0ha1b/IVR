using Domain.Entities;

namespace Application.Interfaces;

/// <summary>
/// Repository interface for managing call session state
/// Typically backed by in-memory cache (MemoryCache or Redis)
/// </summary>
public interface ICallSessionRepository
{
    /// <summary>
    /// Retrieves a call session by CallSid
    /// </summary>
    Task<CallSession?> GetByCallSidAsync(string callSid, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves or updates a call session
    /// </summary>
    Task SaveAsync(CallSession session, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a call session from cache
    /// </summary>
    Task RemoveAsync(string callSid, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a session exists
    /// </summary>
    Task<bool> ExistsAsync(string callSid, CancellationToken cancellationToken = default);
}
