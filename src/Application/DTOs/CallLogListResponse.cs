namespace Application.DTOs;

/// <summary>
/// Response DTO for paginated call log list
/// </summary>
public class CallLogListResponse
{
    public IEnumerable<CallLogDto> CallLogs { get; set; } = new List<CallLogDto>();
    public int TotalCount { get; set; }
    public int Skip { get; set; }
    public int Take { get; set; }
}
