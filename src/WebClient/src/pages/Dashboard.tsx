import React, { useEffect, useState } from 'react';
import { fetchCallLogs } from '../api/callLogsApi';
import { CallLog } from '../types/CallLog';
import CallLogTable from '../components/CallLogTable';
import Statistics from '../components/Statistics';
import Pagination from '../components/Pagination';

const Dashboard: React.FC = () => {
  const [callLogs, setCallLogs] = useState<CallLog[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [currentPage, setCurrentPage] = useState(0);
  const [totalCount, setTotalCount] = useState(0);
  const pageSize = 20;

  const loadCallLogs = async () => {
    try {
      setIsLoading(true);
      setError(null);
      const response = await fetchCallLogs(currentPage * pageSize, pageSize);
      setCallLogs(response.callLogs);
      setTotalCount(response.totalCount);
    } catch (err) {
      setError('Failed to load call logs. Please check if the API server is running.');
      console.error('Error loading call logs:', err);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    loadCallLogs();

    // Auto-refresh every 30 seconds
    const interval = setInterval(loadCallLogs, 30000);
    return () => clearInterval(interval);
  }, [currentPage]);

  const handlePageChange = (page: number) => {
    setCurrentPage(page);
  };

  const handleRefresh = () => {
    loadCallLogs();
  };

  return (
    <div className="space-y-6">
      {/* Header Section */}
      <div className="flex justify-between items-center">
        <div>
          <h2 className="text-2xl font-bold text-gray-900">Call Log Dashboard</h2>
          <p className="text-gray-600 mt-1">
            Real-time monitoring of IVR system interactions
          </p>
        </div>
        <button
          onClick={handleRefresh}
          disabled={isLoading}
          className="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:opacity-50 disabled:cursor-not-allowed"
        >
          <svg
            className={`-ml-1 mr-2 h-5 w-5 ${isLoading ? 'animate-spin' : ''}`}
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"
            />
          </svg>
          Refresh
        </button>
      </div>

      {/* Error Message */}
      {error && (
        <div className="bg-red-50 border-l-4 border-red-400 p-4">
          <div className="flex">
            <div className="flex-shrink-0">
              <svg
                className="h-5 w-5 text-red-400"
                fill="currentColor"
                viewBox="0 0 20 20"
              >
                <path
                  fillRule="evenodd"
                  d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z"
                  clipRule="evenodd"
                />
              </svg>
            </div>
            <div className="ml-3">
              <p className="text-sm text-red-700">{error}</p>
            </div>
          </div>
        </div>
      )}

      {/* Statistics */}
      {!error && <Statistics callLogs={callLogs} />}

      {/* Call Logs Table */}
      <div className="bg-white shadow rounded-lg overflow-hidden">
        <div className="px-6 py-4 border-b border-gray-200">
          <h3 className="text-lg font-medium text-gray-900">Recent Call Logs</h3>
        </div>
        <CallLogTable callLogs={callLogs} isLoading={isLoading} />
        {!isLoading && callLogs.length > 0 && (
          <Pagination
            currentPage={currentPage}
            totalCount={totalCount}
            pageSize={pageSize}
            onPageChange={handlePageChange}
          />
        )}
      </div>
    </div>
  );
};

export default Dashboard;
