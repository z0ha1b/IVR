using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

/// <summary>
/// Controller for retrieving call logs for the dashboard
/// Returns JSON responses
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CallLogsController : ControllerBase
{
    private readonly ICallLogRepository _callLogRepository;
    private readonly ILogger<CallLogsController> _logger;

    public CallLogsController(
        ICallLogRepository callLogRepository,
        ILogger<CallLogsController> logger)
    {
        _callLogRepository = callLogRepository ?? throw new ArgumentNullException(nameof(callLogRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets paginated list of call logs
    /// </summary>
    /// <param name="skip">Number of records to skip (default: 0)</param>
    /// <param name="take">Number of records to take (default: 50, max: 100)</param>
    /// <returns>List of call logs with pagination info</returns>
    [HttpGet]
    [ProducesResponseType(typeof(CallLogListResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<CallLogListResponse>> GetCallLogs(
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50)
    {
        try
        {
            // Validate pagination parameters
            if (skip < 0) skip = 0;
            if (take < 1 || take > 100) take = 50;

            _logger.LogInformation("Retrieving call logs - Skip: {Skip}, Take: {Take}", skip, take);

            var callLogs = await _callLogRepository.GetAllAsync(skip, take);
            var totalCount = await _callLogRepository.GetCountAsync();

            var response = new CallLogListResponse
            {
                CallLogs = callLogs.Select(MapToDto),
                TotalCount = totalCount,
                Skip = skip,
                Take = take
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving call logs");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Gets call logs for a specific call session
    /// </summary>
    /// <param name="callSid">Unique call identifier</param>
    /// <returns>List of call logs for the session</returns>
    [HttpGet("session/{callSid}")]
    [ProducesResponseType(typeof(IEnumerable<CallLogDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<CallLogDto>>> GetCallLogsByCallSid(string callSid)
    {
        try
        {
            _logger.LogInformation("Retrieving call logs for CallSid: {CallSid}", callSid);

            var callLogs = await _callLogRepository.GetByCallSidAsync(callSid);
            var logList = callLogs.ToList();

            if (!logList.Any())
            {
                return NotFound($"No call logs found for CallSid: {callSid}");
            }

            return Ok(logList.Select(MapToDto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving call logs for CallSid: {CallSid}", callSid);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Gets call logs for a specific phone number
    /// </summary>
    /// <param name="phoneNumber">Phone number to search</param>
    /// <returns>List of call logs for the phone number</returns>
    [HttpGet("phone/{phoneNumber}")]
    [ProducesResponseType(typeof(IEnumerable<CallLogDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CallLogDto>>> GetCallLogsByPhoneNumber(string phoneNumber)
    {
        try
        {
            _logger.LogInformation("Retrieving call logs for phone: {PhoneNumber}", phoneNumber);

            var callLogs = await _callLogRepository.GetByPhoneNumberAsync(phoneNumber);
            return Ok(callLogs.Select(MapToDto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving call logs for phone: {PhoneNumber}", phoneNumber);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Maps domain entity to DTO
    /// </summary>
    private static CallLogDto MapToDto(Domain.Entities.CallLog callLog)
    {
        return new CallLogDto
        {
            Id = callLog.Id,
            CallSid = callLog.CallSid,
            CallerNumber = callLog.CallerNumber.Value,
            MenuPath = callLog.MenuPath,
            DigitPressed = callLog.DigitPressed,
            CurrentMenuId = callLog.CurrentMenuId,
            Timestamp = callLog.Timestamp
        };
    }
}
