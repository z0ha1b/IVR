-- =============================================
-- Create CallLogs Table
-- =============================================

USE IVRSystem;
GO

-- Create CallLogs table
CREATE TABLE CallLogs
(
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    CallSid NVARCHAR(100) NOT NULL,
    CallerNumber NVARCHAR(50) NOT NULL,
    MenuPath NVARCHAR(500) NOT NULL,
    DigitPressed NVARCHAR(10) NOT NULL,
    CurrentMenuId NVARCHAR(50) NOT NULL,
    Timestamp DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    -- Add indexes for common queries
    INDEX IX_CallLogs_CallSid (CallSid),
    INDEX IX_CallLogs_CallerNumber (CallerNumber),
    INDEX IX_CallLogs_Timestamp (Timestamp DESC)
);
GO

PRINT 'Table CallLogs created successfully';
GO
