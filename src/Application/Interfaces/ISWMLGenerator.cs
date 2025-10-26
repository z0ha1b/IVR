using Domain.ValueObjects;

namespace Application.Interfaces;

/// <summary>
/// Interface for generating SWML (SignalWire Markup Language) responses
/// </summary>
public interface ISWMLGenerator
{
    /// <summary>
    /// Generates SWML for a menu with gather options
    /// </summary>
    /// <param name="menuState">The current menu state</param>
    /// <param name="actionUrl">The webhook URL to POST digit input</param>
    /// <returns>SWML XML string</returns>
    string GenerateMenuResponse(MenuState menuState, string actionUrl);

    /// <summary>
    /// Generates SWML for an error message
    /// </summary>
    string GenerateErrorResponse(string message);

    /// <summary>
    /// Generates SWML for call termination
    /// </summary>
    string GenerateHangupResponse(string message);
}
