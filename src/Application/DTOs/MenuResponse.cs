namespace Application.DTOs;

/// <summary>
/// Response DTO containing SWML markup for SignalWire
/// </summary>
public class MenuResponse
{
    /// <summary>
    /// The SWML (TwiML) XML content to return to SignalWire
    /// </summary>
    public string SWMLContent { get; set; } = string.Empty;

    /// <summary>
    /// HTTP content type (should be application/xml)
    /// </summary>
    public string ContentType { get; set; } = "application/xml";

    /// <summary>
    /// Current menu ID for tracking
    /// </summary>
    public string MenuId { get; set; } = string.Empty;

    /// <summary>
    /// Success indicator
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Error message if any
    /// </summary>
    public string? ErrorMessage { get; set; }
}
