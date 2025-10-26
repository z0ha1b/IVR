-- =============================================
-- Create IVR System Database
-- =============================================

-- Switch to master database to create new database
USE master;
GO

-- Drop database if it exists (for clean setup)
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'IVRSystem')
BEGIN
    ALTER DATABASE IVRSystem SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE IVRSystem;
END
GO

-- Create the database
CREATE DATABASE IVRSystem;
GO

-- Switch to the new database
USE IVRSystem;
GO

PRINT 'Database IVRSystem created successfully';
GO
