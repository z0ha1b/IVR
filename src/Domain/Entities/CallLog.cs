using Domain.ValueObjects;

namespace Domain.Entities;

/// <summary>
/// Entity representing a log entry for a caller's menu selection
/// Tracks the complete path of user interactions
/// </summary>
public class CallLog
{
    public Guid Id { get; private set; }
    public string CallSid { get; private set; }
    public PhoneNumber CallerNumber { get; private set; }
    public string MenuPath { get; private set; }
    public string DigitPressed { get; private set; }
    public string CurrentMenuId { get; private set; }
    public DateTime Timestamp { get; private set; }

    // Required for ORM
    private CallLog() { }

    public CallLog(
        string callSid,
        PhoneNumber callerNumber,
        string menuPath,
        string digitPressed,
        string currentMenuId)
    {
        Id = Guid.NewGuid();
        CallSid = callSid ?? throw new ArgumentNullException(nameof(callSid));
        CallerNumber = callerNumber ?? throw new ArgumentNullException(nameof(callerNumber));
        MenuPath = menuPath ?? string.Empty;
        DigitPressed = digitPressed ?? throw new ArgumentNullException(nameof(digitPressed));
        CurrentMenuId = currentMenuId ?? throw new ArgumentNullException(nameof(currentMenuId));
        Timestamp = DateTime.UtcNow;
    }

    public void UpdateMenuPath(string newPath)
    {
        MenuPath = newPath ?? throw new ArgumentNullException(nameof(newPath));
    }
}
