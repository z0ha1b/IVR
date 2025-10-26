using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

/// <summary>
/// Controller handling SignalWire webhook requests for IVR operations
/// All endpoints return XML (SWML) content type
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/xml")]
public class IVRController : ControllerBase
{
    private readonly IIVRMenuService _menuService;
    private readonly ILogger<IVRController> _logger;

    public IVRController(
        IIVRMenuService menuService,
        ILogger<IVRController> logger)
    {
        _menuService = menuService ?? throw new ArgumentNullException(nameof(menuService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles initial incoming call from SignalWire
    /// Returns the main menu SWML response
    /// </summary>
    /// <param name="callSid">Unique call identifier from SignalWire</param>
    /// <param name="from">Caller's phone number</param>
    /// <returns>SWML XML response</returns>
    [HttpPost("main")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> HandleIncomingCall(
        [FromForm(Name = "CallSid")] string callSid,
        [FromForm(Name = "From")] string from)
    {
        _logger.LogInformation("Incoming call - CallSid: {CallSid}, From: {From}", callSid, from);

        if (string.IsNullOrWhiteSpace(callSid) || string.IsNullOrWhiteSpace(from))
        {
            _logger.LogWarning("Invalid incoming call request - missing CallSid or From");
            return BadRequest("Invalid request");
        }

        try
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var response = await _menuService.HandleIncomingCallAsync(callSid, from, baseUrl);

            if (!response.IsSuccess)
            {
                _logger.LogWarning("Error handling incoming call: {Error}", response.ErrorMessage);
            }

            return Content(response.SWMLContent, "application/xml");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception handling incoming call");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Handles menu digit input from caller
    /// Processes navigation and returns next menu SWML response
    /// </summary>
    /// <param name="callSid">Unique call identifier</param>
    /// <param name="from">Caller's phone number</param>
    /// <param name="digits">Digit pressed by caller (1, 2, or 3)</param>
    /// <returns>SWML XML response</returns>
    [HttpPost("menu")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> HandleMenuSelection(
        [FromForm(Name = "CallSid")] string callSid,
        [FromForm(Name = "From")] string from,
        [FromForm(Name = "Digits")] string digits)
    {
        _logger.LogInformation(
            "Menu selection - CallSid: {CallSid}, From: {From}, Digits: {Digits}",
            callSid, from, digits);

        if (string.IsNullOrWhiteSpace(callSid) || string.IsNullOrWhiteSpace(from))
        {
            _logger.LogWarning("Invalid menu request - missing CallSid or From");
            return BadRequest("Invalid request");
        }

        try
        {
            var request = new MenuNavigationRequest
            {
                CallSid = callSid,
                From = from,
                Digits = digits ?? string.Empty
            };

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var response = await _menuService.HandleMenuSelectionAsync(request, baseUrl);

            if (!response.IsSuccess)
            {
                _logger.LogWarning("Error handling menu selection: {Error}", response.ErrorMessage);
            }

            return Content(response.SWMLContent, "application/xml");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception handling menu selection");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Webhook for call status callbacks (optional)
    /// Can be used to clean up session when call ends
    /// </summary>
    [HttpPost("status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> HandleCallStatus(
        [FromForm(Name = "CallSid")] string callSid,
        [FromForm(Name = "CallStatus")] string callStatus)
    {
        _logger.LogInformation("Call status - CallSid: {CallSid}, Status: {CallStatus}", callSid, callStatus);

        if (callStatus is "completed" or "busy" or "no-answer" or "canceled" or "failed")
        {
            await _menuService.EndCallSessionAsync(callSid);
        }

        return Ok();
    }
}
