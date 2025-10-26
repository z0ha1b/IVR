using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure.Repositories;

/// <summary>
/// Repository implementation for CallSession using in-memory cache
/// Session data is stored temporarily for the duration of the call
/// </summary>
public class CallSessionRepository : ICallSessionRepository
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<CallSessionRepository> _logger;
    private readonly MemoryCacheEntryOptions _cacheOptions;

    public CallSessionRepository(
        IMemoryCache cache,
        ILogger<CallSessionRepository> logger)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Set cache expiration to 1 hour (typical max call duration)
        _cacheOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(30))
            .SetAbsoluteExpiration(TimeSpan.FromHours(1));
    }

    public Task<CallSession?> GetByCallSidAsync(string callSid, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(callSid))
            throw new ArgumentException("CallSid cannot be empty", nameof(callSid));

        try
        {
            var cacheKey = GetCacheKey(callSid);
            if (_cache.TryGetValue(cacheKey, out string? sessionJson) && sessionJson != null)
            {
                var session = DeserializeSession(sessionJson);
                _logger.LogDebug("Retrieved session for CallSid {CallSid}", callSid);
                return Task.FromResult<CallSession?>(session);
            }

            _logger.LogDebug("Session not found for CallSid {CallSid}", callSid);
            return Task.FromResult<CallSession?>(null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving session for CallSid {CallSid}", callSid);
            throw;
        }
    }

    public Task SaveAsync(CallSession session, CancellationToken cancellationToken = default)
    {
        if (session == null)
            throw new ArgumentNullException(nameof(session));

        try
        {
            var cacheKey = GetCacheKey(session.CallSid);
            var sessionJson = SerializeSession(session);

            _cache.Set(cacheKey, sessionJson, _cacheOptions);
            _logger.LogDebug("Saved session for CallSid {CallSid}", session.CallSid);

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving session for CallSid {CallSid}", session.CallSid);
            throw;
        }
    }

    public Task RemoveAsync(string callSid, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(callSid))
            throw new ArgumentException("CallSid cannot be empty", nameof(callSid));

        try
        {
            var cacheKey = GetCacheKey(callSid);
            _cache.Remove(cacheKey);
            _logger.LogDebug("Removed session for CallSid {CallSid}", callSid);

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing session for CallSid {CallSid}", callSid);
            throw;
        }
    }

    public Task<bool> ExistsAsync(string callSid, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(callSid))
            throw new ArgumentException("CallSid cannot be empty", nameof(callSid));

        var cacheKey = GetCacheKey(callSid);
        var exists = _cache.TryGetValue(cacheKey, out _);

        return Task.FromResult(exists);
    }

    private static string GetCacheKey(string callSid) => $"CallSession:{callSid}";

    /// <summary>
    /// Serializes CallSession to JSON for storage
    /// </summary>
    private string SerializeSession(CallSession session)
    {
        var dto = new CallSessionDto
        {
            CallSid = session.CallSid,
            CallerNumber = session.CallerNumber.Value,
            MenuStack = session.MenuStack.Select(m => new MenuStateDto
            {
                MenuId = m.MenuId,
                Level = (int)m.Level,
                Message = m.Message,
                ParentMenuId = m.ParentMenuId
            }).ToList(),
            CurrentMenuId = session.CurrentMenuId,
            StartTime = session.StartTime,
            LastActivityTime = session.LastActivityTime
        };

        return JsonSerializer.Serialize(dto);
    }

    /// <summary>
    /// Deserializes CallSession from JSON
    /// </summary>
    private CallSession? DeserializeSession(string json)
    {
        var dto = JsonSerializer.Deserialize<CallSessionDto>(json);
        if (dto == null) return null;

        var phoneNumber = new Domain.ValueObjects.PhoneNumber(dto.CallerNumber);

        // Reconstruct menu stack
        var menuStates = dto.MenuStack.Select(m => new Domain.ValueObjects.MenuState(
            m.MenuId,
            (Domain.Enums.MenuLevel)m.Level,
            m.Message,
            m.ParentMenuId
        )).ToList();

        // Create session using reflection (since constructor is specific)
        var session = (CallSession)Activator.CreateInstance(
            typeof(CallSession),
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
            null,
            new object[] { dto.CallSid, phoneNumber, menuStates.First() },
            null)!;

        // Restore the full stack
        var stackProperty = typeof(CallSession).GetProperty(nameof(CallSession.MenuStack));
        var stack = new Stack<Domain.ValueObjects.MenuState>(menuStates.AsEnumerable().Reverse());
        stackProperty?.SetValue(session, stack);

        return session;
    }

    /// <summary>
    /// DTOs for JSON serialization
    /// </summary>
    private class CallSessionDto
    {
        public string CallSid { get; set; } = string.Empty;
        public string CallerNumber { get; set; } = string.Empty;
        public List<MenuStateDto> MenuStack { get; set; } = new();
        public string CurrentMenuId { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime LastActivityTime { get; set; }
    }

    private class MenuStateDto
    {
        public string MenuId { get; set; } = string.Empty;
        public int Level { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? ParentMenuId { get; set; }
    }
}
