-- Simple test migration: add a nullable bit column MYID_TEST_FLAG to QRTZ_JOB_DETAILS if it doesn't exist.
-- Uses the ${schema} variable (DbUp will substitute) — keep the token style consistent with your migrator.
SET NOCOUNT ON;

DECLARE @schema_name sysname = N'$schema$';
DECLARE @sql nvarchar(max);

IF NOT EXISTS (
    SELECT 1
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = @schema_name
      AND TABLE_NAME   = 'QRTZ_JOB_DETAILS'
      AND COLUMN_NAME  = 'MYID_TEST_FLAG'
)
BEGIN
    SET @sql = N'ALTER TABLE ' + QUOTENAME(@schema_name) + N'.' + QUOTENAME(N'QRTZ_JOB_DETAILS')
             + N' ADD [MYID_TEST_FLAG] bit NULL;';
    EXEC(@sql);
END;
GO