using Microsoft.Data.SqlClient;

namespace StudentManagementSystem.Data
{
    public static class DatabaseInitializer
    {
        public static async Task InitializeAsync(IConfiguration configuration, ILogger logger)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");
            }

            var connectionBuilder = new SqlConnectionStringBuilder(connectionString);
            if (string.IsNullOrWhiteSpace(connectionBuilder.InitialCatalog))
            {
                throw new InvalidOperationException("Connection string must include a database name.");
            }

            var databaseName = connectionBuilder.InitialCatalog;
            var masterBuilder = new SqlConnectionStringBuilder(connectionString)
            {
                InitialCatalog = "master"
            };

            logger.LogInformation("Starting database initialization for {DatabaseName}", databaseName);

            await EnsureDatabaseExistsAsync(masterBuilder.ConnectionString, databaseName);
            await EnsureSchemaAndStoredProceduresAsync(connectionString);
            await EnsureMinimumSeedDataAsync(connectionString, 500);

            logger.LogInformation("Database initialization completed for {DatabaseName}", databaseName);
        }

        private static async Task EnsureDatabaseExistsAsync(string masterConnectionString, string databaseName)
        {
            await using var connection = new SqlConnection(masterConnectionString);
            await connection.OpenAsync();

            const string createDbSql = @"
IF DB_ID(@DatabaseName) IS NULL
BEGIN
    DECLARE @sql NVARCHAR(MAX) = N'CREATE DATABASE [' + @DatabaseName + N']';
    EXEC(@sql);
END";

            await using var command = new SqlCommand(createDbSql, connection);
            command.Parameters.AddWithValue("@DatabaseName", databaseName);
            await command.ExecuteNonQueryAsync();
        }

        private static async Task EnsureSchemaAndStoredProceduresAsync(string appConnectionString)
        {
            await using var connection = new SqlConnection(appConnectionString);
            await connection.OpenAsync();

            foreach (var sql in GetSetupCommands())
            {
                await using var command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
            }
        }

        private static async Task EnsureMinimumSeedDataAsync(string appConnectionString, int minimumRowCount)
        {
            await using var connection = new SqlConnection(appConnectionString);
            await connection.OpenAsync();

            const string seedSql = @"
DECLARE @TargetCount INT = @MinimumRowCount;
DECLARE @CurrentCount INT = (SELECT COUNT(1) FROM dbo.Students);

IF @CurrentCount < @TargetCount
BEGIN
    DECLARE @i INT = @CurrentCount + 1;

    WHILE @i <= @TargetCount
    BEGIN
        BEGIN TRY
            INSERT INTO dbo.Students
            (
                FirstName,
                LastName,
                Email,
                PhoneNumber,
                DateOfBirth,
                Address,
                EnrollmentDate,
                Status,
                CreatedAt,
                UpdatedAt
            )
            VALUES
            (
                N'Student' + RIGHT('0000' + CAST(@i AS NVARCHAR(10)), 4),
                N'Demo',
                N'student' + CAST(@i AS NVARCHAR(10)) + N'@demo.local',
                N'01' + RIGHT('0000000000' + CAST(1000000000 + @i AS NVARCHAR(15)), 10),
                DATEADD(DAY, -(6200 + @i), CAST(GETDATE() AS DATE)),
                N'Address ' + CAST(@i AS NVARCHAR(10)),
                DATEADD(DAY, -(@i % 365), GETDATE()),
                CASE WHEN @i % 10 = 0 THEN N'Inactive' ELSE N'Active' END,
                GETDATE(),
                GETDATE()
            );
        END TRY
        BEGIN CATCH
            -- Skip duplicate key violations and continue
        END CATCH

        SET @i = @i + 1;
    END
END";

            await using var command = new SqlCommand(seedSql, connection);
            command.Parameters.AddWithValue("@MinimumRowCount", minimumRowCount);
            await command.ExecuteNonQueryAsync();
        }

        private static IEnumerable<string> GetSetupCommands()
        {
            return new[]
            {
                @"
IF OBJECT_ID('dbo.Students', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Students
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        FirstName NVARCHAR(50) NOT NULL,
        LastName NVARCHAR(50) NOT NULL,
        Email NVARCHAR(100) NOT NULL UNIQUE,
        PhoneNumber NVARCHAR(20) NOT NULL UNIQUE,
        DateOfBirth DATETIME NOT NULL,
        Address NVARCHAR(200) NOT NULL,
        EnrollmentDate DATETIME NOT NULL CONSTRAINT DF_Students_EnrollmentDate DEFAULT GETDATE(),
        Status NVARCHAR(20) NOT NULL CONSTRAINT DF_Students_Status DEFAULT 'Active',
        CreatedAt DATETIME NOT NULL CONSTRAINT DF_Students_CreatedAt DEFAULT GETDATE(),
        UpdatedAt DATETIME NOT NULL CONSTRAINT DF_Students_UpdatedAt DEFAULT GETDATE()
    );
END",
                @"
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Students_Email' AND object_id = OBJECT_ID('dbo.Students'))
BEGIN
    CREATE INDEX IX_Students_Email ON dbo.Students(Email);
END",
                @"
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Students_PhoneNumber' AND object_id = OBJECT_ID('dbo.Students'))
BEGIN
    CREATE INDEX IX_Students_PhoneNumber ON dbo.Students(PhoneNumber);
END",
                @"
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Students_FirstName_LastName' AND object_id = OBJECT_ID('dbo.Students'))
BEGIN
    CREATE INDEX IX_Students_FirstName_LastName ON dbo.Students(FirstName, LastName);
END",
                @"
CREATE OR ALTER PROCEDURE dbo.sp_GetAllStudents
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        FirstName,
        LastName,
        Email,
        PhoneNumber,
        DateOfBirth,
        Address,
        EnrollmentDate,
        Status
    FROM dbo.Students
    ORDER BY Id ASC;
END",
                @"
CREATE OR ALTER PROCEDURE dbo.sp_GetStudentById
    @StudentId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        FirstName,
        LastName,
        Email,
        PhoneNumber,
        DateOfBirth,
        Address,
        EnrollmentDate,
        Status
    FROM dbo.Students
    WHERE Id = @StudentId;
END",
                @"
CREATE OR ALTER PROCEDURE dbo.sp_SearchStudents
    @SearchTerm NVARCHAR(100) = ''
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        FirstName,
        LastName,
        Email,
        PhoneNumber,
        DateOfBirth,
        Address,
        EnrollmentDate,
        Status
    FROM dbo.Students
    WHERE
        (@SearchTerm = '' OR
         FirstName LIKE '%' + @SearchTerm + '%' OR
         LastName LIKE '%' + @SearchTerm + '%' OR
         Email LIKE '%' + @SearchTerm + '%' OR
         PhoneNumber LIKE '%' + @SearchTerm + '%')
    ORDER BY Id ASC;
END",
                @"
CREATE OR ALTER PROCEDURE dbo.sp_GetStudentsPaginated
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(100) = ''
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @SkipRows INT = (@PageNumber - 1) * @PageSize;

    SELECT
        Id,
        FirstName,
        LastName,
        Email,
        PhoneNumber,
        DateOfBirth,
        Address,
        EnrollmentDate,
        Status
    FROM dbo.Students
    WHERE
        (@SearchTerm = '' OR
         FirstName LIKE '%' + @SearchTerm + '%' OR
         LastName LIKE '%' + @SearchTerm + '%' OR
         Email LIKE '%' + @SearchTerm + '%' OR
         PhoneNumber LIKE '%' + @SearchTerm + '%')
    ORDER BY Id ASC
    OFFSET @SkipRows ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END",
                @"
CREATE OR ALTER PROCEDURE dbo.sp_GetStudentCount
    @SearchTerm NVARCHAR(100) = ''
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(1)
    FROM dbo.Students
    WHERE
        (@SearchTerm = '' OR
         FirstName LIKE '%' + @SearchTerm + '%' OR
         LastName LIKE '%' + @SearchTerm + '%' OR
         Email LIKE '%' + @SearchTerm + '%' OR
         PhoneNumber LIKE '%' + @SearchTerm + '%');
END",
                @"
CREATE OR ALTER PROCEDURE dbo.sp_CreateStudent
    @FirstName NVARCHAR(50),
    @LastName NVARCHAR(50),
    @Email NVARCHAR(100),
    @PhoneNumber NVARCHAR(20),
    @DateOfBirth DATETIME,
    @Address NVARCHAR(200),
    @Status NVARCHAR(20) = 'Active'
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Students
    (
        FirstName,
        LastName,
        Email,
        PhoneNumber,
        DateOfBirth,
        Address,
        EnrollmentDate,
        Status,
        CreatedAt,
        UpdatedAt
    )
    VALUES
    (
        @FirstName,
        @LastName,
        @Email,
        @PhoneNumber,
        @DateOfBirth,
        @Address,
        GETDATE(),
        @Status,
        GETDATE(),
        GETDATE()
    );

    SELECT CAST(SCOPE_IDENTITY() AS INT);
END",
                @"
CREATE OR ALTER PROCEDURE dbo.sp_UpdateStudent
    @StudentId INT,
    @FirstName NVARCHAR(50),
    @LastName NVARCHAR(50),
    @Email NVARCHAR(100),
    @PhoneNumber NVARCHAR(20),
    @DateOfBirth DATETIME,
    @Address NVARCHAR(200),
    @Status NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Students
    SET
        FirstName = @FirstName,
        LastName = @LastName,
        Email = @Email,
        PhoneNumber = @PhoneNumber,
        DateOfBirth = @DateOfBirth,
        Address = @Address,
        Status = @Status,
        UpdatedAt = GETDATE()
    WHERE Id = @StudentId;
END",
                @"
CREATE OR ALTER PROCEDURE dbo.sp_DeleteStudent
    @StudentId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.Students
    WHERE Id = @StudentId;
END"
            };
        }
    }
}
