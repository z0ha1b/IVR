import React from 'react';

const Header: React.FC = () => {
  return (
    <header className="bg-blue-600 text-white shadow-lg">
      <div className="container mx-auto px-4 py-6">
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-3xl font-bold">IVR System Dashboard</h1>
            <p className="text-blue-100 mt-1">Monitor and analyze call interactions</p>
          </div>
          <div className="flex items-center space-x-4">
            <div className="text-right">
              <p className="text-sm text-blue-100">SignalWire Integration</p>
              <p className="text-xs text-blue-200">Real-time Call Logging</p>
            </div>
          </div>
        </div>
      </div>
    </header>
  );
};

export default Header;
