namespace Application.DTOs;

/// <summary>
/// Request DTO for menu navigation containing caller input
/// </summary>
public class MenuNavigationRequest
{
    /// <summary>
    /// Unique call identifier from SignalWire
    /// </summary>
    public string CallSid { get; set; } = string.Empty;

    /// <summary>
    /// Caller's phone number
    /// </summary>
    public string From { get; set; } = string.Empty;

    /// <summary>
    /// The digit pressed by the caller (1, 2, or 3)
    /// </summary>
    public string Digits { get; set; } = string.Empty;

    /// <summary>
    /// Optional: Current menu ID for validation
    /// </summary>
    public string? CurrentMenuId { get; set; }
}
