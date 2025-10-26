using Application.DTOs;

namespace Application.Interfaces;

/// <summary>
/// Service interface for managing IVR menu navigation and state
/// </summary>
public interface IIVRMenuService
{
    /// <summary>
    /// Handles an incoming call and returns the main menu
    /// </summary>
    Task<MenuResponse> HandleIncomingCallAsync(
        string callSid,
        string callerNumber,
        string baseUrl,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Processes a menu selection and returns the next menu or action
    /// </summary>
    Task<MenuResponse> HandleMenuSelectionAsync(
        MenuNavigationRequest request,
        string baseUrl,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Ends a call session and cleans up state
    /// </summary>
    Task EndCallSessionAsync(string callSid, CancellationToken cancellationToken = default);
}
