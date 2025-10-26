/**
 * Represents a single call log entry
 */
export interface CallLog {
  id: string;
  callSid: string;
  callerNumber: string;
  menuPath: string;
  digitPressed: string;
  currentMenuId: string;
  timestamp: string;
}

/**
 * Response type for paginated call log list
 */
export interface CallLogListResponse {
  callLogs: CallLog[];
  totalCount: number;
  skip: number;
  take: number;
}
