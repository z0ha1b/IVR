import axios from 'axios';
import { CallLog, CallLogListResponse } from '../types/CallLog';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000';

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

/**
 * Fetches paginated call logs from the API
 */
export const fetchCallLogs = async (
  skip: number = 0,
  take: number = 50
): Promise<CallLogListResponse> => {
  const response = await apiClient.get<CallLogListResponse>('/api/calllogs', {
    params: { skip, take },
  });
  return response.data;
};

/**
 * Fetches call logs for a specific call session
 */
export const fetchCallLogsByCallSid = async (callSid: string): Promise<CallLog[]> => {
  const response = await apiClient.get<CallLog[]>(`/api/calllogs/session/${callSid}`);
  return response.data;
};

/**
 * Fetches call logs for a specific phone number
 */
export const fetchCallLogsByPhoneNumber = async (phoneNumber: string): Promise<CallLog[]> => {
  const response = await apiClient.get<CallLog[]>(`/api/calllogs/phone/${phoneNumber}`);
  return response.data;
};

/**
 * Health check endpoint
 */
export const checkHealth = async (): Promise<{ status: string; timestamp: string }> => {
  const response = await apiClient.get('/health');
  return response.data;
};

export default apiClient;
