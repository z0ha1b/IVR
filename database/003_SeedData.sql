-- =============================================
-- Seed Sample Data for Testing
-- =============================================

USE IVRSystem;
GO

-- Insert sample call logs for testing the dashboard
INSERT INTO CallLogs (Id, CallSid, CallerNumber, MenuPath, DigitPressed, CurrentMenuId, Timestamp)
VALUES
    (NEWID(), 'CA1234567890abcdef', '+15551234567', 'MAIN', 'INCOMING', 'MAIN', DATEADD(MINUTE, -30, GETUTCDATE())),
    (NEWID(), 'CA1234567890abcdef', '+15551234567', 'MAIN', '1', 'SALES', DATEADD(MINUTE, -29, GETUTCDATE())),
    (NEWID(), 'CA1234567890abcdef', '+15551234567', 'MAIN > SALES', '1', 'SALES_PRODUCT', DATEADD(MINUTE, -28, GETUTCDATE())),
    (NEWID(), 'CA1234567890abcdef', '+15551234567', 'MAIN > SALES', '3', 'SALES', DATEADD(MINUTE, -27, GETUTCDATE())),

    (NEWID(), 'CA0987654321fedcba', '+15559876543', 'MAIN', 'INCOMING', 'MAIN', DATEADD(MINUTE, -20, GETUTCDATE())),
    (NEWID(), 'CA0987654321fedcba', '+15559876543', 'MAIN', '2', 'SUPPORT', DATEADD(MINUTE, -19, GETUTCDATE())),
    (NEWID(), 'CA0987654321fedcba', '+15559876543', 'MAIN > SUPPORT', '1', 'SUPPORT_TECHNICAL', DATEADD(MINUTE, -18, GETUTCDATE())),

    (NEWID(), 'CAabcdef1234567890', '+15555555555', 'MAIN', 'INCOMING', 'MAIN', DATEADD(MINUTE, -10, GETUTCDATE())),
    (NEWID(), 'CAabcdef1234567890', '+15555555555', 'MAIN', '3', 'BILLING', DATEADD(MINUTE, -9, GETUTCDATE())),
    (NEWID(), 'CAabcdef1234567890', '+15555555555', 'MAIN > BILLING', '2', 'BILLING_INVOICE', DATEADD(MINUTE, -8, GETUTCDATE()));

GO

PRINT 'Sample data inserted successfully';
PRINT 'Total records: ' + CAST((SELECT COUNT(*) FROM CallLogs) AS NVARCHAR(10));
GO
