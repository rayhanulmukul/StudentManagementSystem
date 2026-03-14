# Student Management System

Student Management System is an ASP.NET Core MVC web application for managing student records with SQL Server stored procedures. The application supports full CRUD operations, searchable student listings, paginated results, and automatic database setup on startup.

## Overview

This repository contains a single MVC application:

- Frontend: ASP.NET Core MVC with Razor views
- Backend: ASP.NET Core on .NET 10
- Database: SQL Server LocalDB by default
- Data access: Stored procedures via `Microsoft.Data.SqlClient`

The application automatically creates the database, table, indexes, stored procedures, and demo seed data when it starts.

## Features

- Create, view, edit, and delete student records
- Search students by first name, last name, email, or phone number
- Paginated student listing with 10 records per page
- Session-based persistence for active search term and current page
- Automatic SQL database initialization at application startup
- Demo data seeding up to at least 500 student rows
- Validation for required fields, email format, and 11-digit phone number

## Student Data

Each student record includes:

- First name
- Last name
- Email
- Phone number
- Date of birth
- Address
- Enrollment date
- Status

Supported status values used by the UI are:

- Active
- Inactive
- Graduated

## Technology Stack

- .NET 10
- ASP.NET Core MVC
- Razor Views
- SQL Server LocalDB / SQL Server
- Bootstrap 5
- `Microsoft.Data.SqlClient`

## Project Structure

```text
StudentManagementSystem/
	README.md
	SETUP.md
	StudentManagementSystem.slnx
	StudentManagementSystem/
		Program.cs
		appsettings.json
		Controllers/
		Data/
		Models/
		Services/
		Views/
		wwwroot/
```

Main application project:

`StudentManagementSystem/StudentManagementSystem.csproj`

## Prerequisites

Before running the project, install:

- .NET 10 SDK
- SQL Server LocalDB or a reachable SQL Server instance
- Visual Studio 2022, Visual Studio Code, or another .NET-compatible editor

Default database connection:

```json
{
	"ConnectionStrings": {
		"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=StudentManagementDb;Trusted_Connection=true;"
	}
}
```

## Getting Started

From the repository root:

```bash
cd StudentManagementSystem
dotnet restore
dotnet build
dotnet run
```

Default development URLs:

- http://localhost:5103
- https://localhost:7080

Open the student management screen:

- http://localhost:5103/Students

## How Startup Works

On application startup, the app runs `DatabaseInitializer.InitializeAsync(...)` before serving requests.

Startup initialization does the following:

- Reads the `DefaultConnection` string from `appsettings.json`
- Creates the target database if it does not exist
- Ensures the `Students` table exists
- Ensures required indexes exist
- Creates or updates the stored procedures used by the app
- Seeds demo data until the database contains at least 500 student records

Because of this, you normally do not need to execute SQL scripts manually for local development.

## Stored Procedures Used

The application uses stored procedures for database operations, including:

- `sp_GetAllStudents`
- `sp_GetStudentById`
- `sp_SearchStudents`
- `sp_GetStudentsPaginated`
- `sp_GetStudentCount`
- `sp_CreateStudent`
- `sp_UpdateStudent`
- `sp_DeleteStudent`

Reference script:

- `StudentManagementSystem/Data/StoredProcedures.sql`

Note: runtime initialization is driven by `StudentManagementSystem/Data/DatabaseInitializer.cs`, which is the authoritative setup path for the current application.

## Configuration

Application configuration files:

- `StudentManagementSystem/appsettings.json`
- `StudentManagementSystem/appsettings.Development.json`
- `StudentManagementSystem/Properties/launchSettings.json`

If you want to use SQL Server Express or another SQL Server instance, update `DefaultConnection` in `appsettings.json`.

Example for a named SQL Server instance:

```json
{
	"ConnectionStrings": {
		"DefaultConnection": "Server=.\\SQLEXPRESS;Database=StudentManagementDb;Trusted_Connection=true;TrustServerCertificate=true;"
	}
}
```

## Validation Rules

The application enforces the following validation rules for student records:

- First name is required and limited to 50 characters
- Last name is required and limited to 50 characters
- Email is required and must be a valid email address
- Phone number is required and must contain exactly 11 digits
- Address is required and limited to 200 characters
- Date of birth must be in the past

## Typical User Flow

1. Start the application.
2. Database initialization runs automatically.
3. Navigate to `/Students`.
4. Browse seeded student data.
5. Search, paginate, create, update, view, or delete records.

## Manual SQL Setup

Manual SQL execution is optional and typically unnecessary.

If you explicitly want to run the SQL script yourself, use:

```bash
cd StudentManagementSystem
sqlcmd -S "(localdb)\\mssqllocaldb" -i "Data/StoredProcedures.sql"
```

## Troubleshooting

### LocalDB is not installed or not running

```bash
sqllocaldb info mssqllocaldb
sqllocaldb start mssqllocaldb
```

If `sqllocaldb` is unavailable, install SQL Server Express LocalDB or update the connection string to a SQL Server instance you already have.

### Port already in use

Run the app on another URL:

```bash
cd StudentManagementSystem
dotnet run --urls "http://localhost:5200"
```

### HTTPS certificate warning

For local development:

```bash
dotnet dev-certs https --trust
```

### Build fails because the executable is locked

If you see errors such as `MSB3021` or `MSB3027`, stop any running instance of the application and build again.

### Database initialization fails

Check the following:

- `DefaultConnection` is present and correct
- SQL Server or LocalDB is reachable
- The configured database user has permission to create a database
- Startup logs include the `DatabaseInitializer` messages

## Verification Checklist

Use this checklist after setup:

- `dotnet restore` completes successfully
- `dotnet build` completes successfully
- `dotnet run` starts the application
- `http://localhost:5103/Students` loads
- Student records are visible
- Search works
- Pagination works
- Create, edit, details, and delete flows work

## Additional Notes

- Session state is enabled and used to preserve the student list search term and current page between requests.
- The app redirects to the `Students` listing for the main student management workflow.
- `SETUP.md` contains a setup-focused version of the same environment guidance.