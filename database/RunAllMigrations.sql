-- =============================================
-- Run All Database Migrations
-- Execute this script to set up the complete database
-- =============================================

PRINT '===============================================';
PRINT 'Starting IVR System Database Setup';
PRINT '===============================================';
PRINT '';

-- Step 1: Create Database
PRINT 'Step 1: Creating database...';
:r 001_CreateDatabase.sql
PRINT '';

-- Step 2: Create Tables
PRINT 'Step 2: Creating tables...';
:r 002_CreateCallLogsTable.sql
PRINT '';

-- Step 3: Seed Data
PRINT 'Step 3: Inserting sample data...';
:r 003_SeedData.sql
PRINT '';

PRINT '===============================================';
PRINT 'Database Setup Completed Successfully!';
PRINT '===============================================';
