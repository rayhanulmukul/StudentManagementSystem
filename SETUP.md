# Student Management System - Setup Guide

This guide reflects the current repository: a single ASP.NET Core MVC app with SQL Server LocalDB. There is no Angular project in this workspace.

## Prerequisites

- .NET 10 SDK (project targets `net10.0`)
- SQL Server LocalDB (`(localdb)\\mssqllocaldb`) or SQL Server Express
- Visual Studio Code or Visual Studio

## Project Structure

From repository root:

```text
StudentManagementSystem/
  SETUP.md
  StudentManagementSystem.slnx
  StudentManagementSystem/
    Program.cs
    appsettings.json
    Data/
      DatabaseInitializer.cs
      StoredProcedures.sql
```

The runnable project file is:

`StudentManagementSystem/StudentManagementSystem.csproj`

## Quick Start (Recommended)

1. Open a terminal at repository root.
2. Move into the app folder.
3. Restore, build, and run.

```bash
cd StudentManagementSystem
dotnet restore
dotnet build
dotnet run
```

Default development URLs (from launch settings):

- http://localhost:5103
- https://localhost:7080

Open the students page:

- http://localhost:5103/Students

## Database Behavior

No manual SQL script execution is required for normal local setup.

At startup, `DatabaseInitializer` automatically:

- Creates `StudentManagementDb` if it does not exist.
- Ensures `Students` table and required indexes exist.
- Creates or updates stored procedures.
- Seeds demo data up to at least 500 rows.

If LocalDB is available, first run should prepare the database automatically.

## Optional: Manual SQL Script Execution

Only use this if you explicitly want to run the SQL script yourself.

```bash
cd StudentManagementSystem
sqlcmd -S (localdb)\\mssqllocaldb -i "Data/StoredProcedures.sql"
```

## Configuration

Connection string location:

- `StudentManagementSystem/appsettings.json`

Default value:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=StudentManagementDb;Trusted_Connection=true;"
  }
}
```

If you use SQL Server Express or another instance, update `DefaultConnection` accordingly.

## Verification Checklist

- `dotnet build` succeeds.
- App starts with `dotnet run`.
- http://localhost:5103/Students loads.
- Student list appears (seeded data expected).
- Create, edit, details, and delete flows work.
- Search and pagination work on the students list.

## Common Issues

### LocalDB not started or not installed

```bash
sqllocaldb start mssqllocaldb
sqllocaldb info mssqllocaldb
```

If `sqllocaldb` is not found, install SQL Server Express LocalDB.

### Port already in use

Use `--urls` to override:

```bash
cd StudentManagementSystem
dotnet run --urls "http://localhost:5200"
```

### TLS/HTTPS certificate warning

For local development, trust the dev certificate:

```bash
dotnet dev-certs https --trust
```

### Database initialization fails

- Verify `DefaultConnection` in `appsettings.json`.
- Confirm SQL Server/LocalDB is reachable.
- Check console logs for `DatabaseInitializer` messages.

## Notes

- `Data/StoredProcedures.sql` is kept for manual setup/reference.
- Runtime setup is driven by `Data/DatabaseInitializer.cs` and executes on app startup.
