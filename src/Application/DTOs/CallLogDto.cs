namespace Application.DTOs;

/// <summary>
/// Data transfer object for call log information
/// </summary>
public class CallLogDto
{
    public Guid Id { get; set; }
    public string CallSid { get; set; } = string.Empty;
    public string CallerNumber { get; set; } = string.Empty;
    public string MenuPath { get; set; } = string.Empty;
    public string DigitPressed { get; set; } = string.Empty;
    public string CurrentMenuId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
