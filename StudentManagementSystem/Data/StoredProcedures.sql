-- SQL Server Stored Procedures for Student Management System
-- Execute this script to create the database and all stored procedures

-- Create Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'StudentManagementDb')
BEGIN
    CREATE DATABASE StudentManagementDb;
END

USE StudentManagementDb;

-- Create Students Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Students')
BEGIN
    CREATE TABLE Students (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        FirstName NVARCHAR(50) NOT NULL,
        LastName NVARCHAR(50) NOT NULL,
        Email NVARCHAR(100) NOT NULL UNIQUE,
        PhoneNumber NVARCHAR(20) NOT NULL UNIQUE,
        DateOfBirth DATETIME NOT NULL,
        Address NVARCHAR(200) NOT NULL,
        EnrollmentDate DATETIME NOT NULL DEFAULT GETDATE(),
        Status NVARCHAR(20) NOT NULL DEFAULT 'Active',
        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME NOT NULL DEFAULT GETDATE()
    );

    -- Create indexes for better query performance
    CREATE INDEX IX_Students_Email ON Students(Email);
    CREATE INDEX IX_Students_PhoneNumber ON Students(PhoneNumber);
    CREATE INDEX IX_Students_Status ON Students(Status);
    CREATE INDEX IX_Students_FirstName ON Students(FirstName);
    CREATE INDEX IX_Students_LastName ON Students(LastName);
END

-- Stored Procedure: Get All Students
IF OBJECT_ID('sp_GetAllStudents', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetAllStudents;

CREATE PROCEDURE sp_GetAllStudents
AS
BEGIN
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
    FROM Students
    ORDER BY Id ASC;
END;

-- Stored Procedure: Get Student By ID
IF OBJECT_ID('sp_GetStudentById', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetStudentById;

CREATE PROCEDURE sp_GetStudentById
    @StudentId INT
AS
BEGIN
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
    FROM Students
    WHERE Id = @StudentId;
END;

-- Stored Procedure: Search Students
IF OBJECT_ID('sp_SearchStudents', 'P') IS NOT NULL
    DROP PROCEDURE sp_SearchStudents;

CREATE PROCEDURE sp_SearchStudents
    @SearchTerm NVARCHAR(100) = ''
AS
BEGIN
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
    FROM Students
    WHERE 
        (@SearchTerm = '' OR 
         FirstName LIKE '%' + @SearchTerm + '%' OR
         LastName LIKE '%' + @SearchTerm + '%' OR
         Email LIKE '%' + @SearchTerm + '%' OR
         PhoneNumber LIKE '%' + @SearchTerm + '%')
    ORDER BY Id ASC;
END;

-- Stored Procedure: Get Students with Pagination
IF OBJECT_ID('sp_GetStudentsPaginated', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetStudentsPaginated;

CREATE PROCEDURE sp_GetStudentsPaginated
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(100) = ''
AS
BEGIN
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
    FROM Students
    WHERE 
        (@SearchTerm = '' OR 
         FirstName LIKE '%' + @SearchTerm + '%' OR
         LastName LIKE '%' + @SearchTerm + '%' OR
         Email LIKE '%' + @SearchTerm + '%' OR
         PhoneNumber LIKE '%' + @SearchTerm + '%')
    ORDER BY Id ASC
    OFFSET @SkipRows ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;

-- Stored Procedure: Get Student Count
IF OBJECT_ID('sp_GetStudentCount', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetStudentCount;

CREATE PROCEDURE sp_GetStudentCount
    @SearchTerm NVARCHAR(100) = ''
AS
BEGIN
    SELECT COUNT(*)
    FROM Students
    WHERE 
        (@SearchTerm = '' OR 
         FirstName LIKE '%' + @SearchTerm + '%' OR
         LastName LIKE '%' + @SearchTerm + '%' OR
         Email LIKE '%' + @SearchTerm + '%' OR
         PhoneNumber LIKE '%' + @SearchTerm + '%');
END;

-- Stored Procedure: Create Student
IF OBJECT_ID('sp_CreateStudent', 'P') IS NOT NULL
    DROP PROCEDURE sp_CreateStudent;

CREATE PROCEDURE sp_CreateStudent
    @FirstName NVARCHAR(50),
    @LastName NVARCHAR(50),
    @Email NVARCHAR(100),
    @PhoneNumber NVARCHAR(20),
    @DateOfBirth DATETIME,
    @Address NVARCHAR(200),
    @Status NVARCHAR(20) = 'Active'
AS
BEGIN
    BEGIN TRY
        INSERT INTO Students (FirstName, LastName, Email, PhoneNumber, DateOfBirth, Address, Status)
        VALUES (@FirstName, @LastName, @Email, @PhoneNumber, @DateOfBirth, @Address, @Status);
        
        SELECT SCOPE_IDENTITY() AS Id;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;

-- Stored Procedure: Update Student
IF OBJECT_ID('sp_UpdateStudent', 'P') IS NOT NULL
    DROP PROCEDURE sp_UpdateStudent;

CREATE PROCEDURE sp_UpdateStudent
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
    BEGIN TRY
        UPDATE Students
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
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;

-- Stored Procedure: Delete Student
IF OBJECT_ID('sp_DeleteStudent', 'P') IS NOT NULL
    DROP PROCEDURE sp_DeleteStudent;

CREATE PROCEDURE sp_DeleteStudent
    @StudentId INT
AS
BEGIN
    BEGIN TRY
        DELETE FROM Students
        WHERE Id = @StudentId;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;

-- Stored Procedure: Get Active Students
IF OBJECT_ID('sp_GetActiveStudents', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetActiveStudents;

CREATE PROCEDURE sp_GetActiveStudents
AS
BEGIN
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
    FROM Students
    WHERE Status = 'Active'
    ORDER BY FirstName ASC;
END;

-- Stored Procedure: Get Students by Status
IF OBJECT_ID('sp_GetStudentsByStatus', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetStudentsByStatus;

CREATE PROCEDURE sp_GetStudentsByStatus
    @Status NVARCHAR(20)
AS
BEGIN
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
    FROM Students
    WHERE Status = @Status
    ORDER BY FirstName ASC;
END;

-- Stored Procedure: Truncate Table (for development/testing)
IF OBJECT_ID('sp_TruncateStudents', 'P') IS NOT NULL
    DROP PROCEDURE sp_TruncateStudents;

CREATE PROCEDURE sp_TruncateStudents
AS
BEGIN
    TRUNCATE TABLE Students;
END;

PRINT 'All stored procedures created successfully!';
