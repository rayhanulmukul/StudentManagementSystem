# Student Management System - ASP.NET MVC Backend

Complete ASP.NET MVC application for managing student information with SQL Server database and stored procedures.

## 🏗️ Architecture

```
Controllers/ 
  └─ StudentsController.cs    (CRUD operations)
Services/
  └─ StudentService.cs        (Business logic)
Data/
  ├─ IStudentRepository.cs    (Data access interface)
  ├─ StudentRepository.cs     (Database operations)
  └─ StoredProcedures.sql     (SQL scripts)
Models/
  └─ Student.cs               (Entity model)
Views/Students/
  ├─ Index.cshtml            (List with search & pagination)
  ├─ Create.cshtml           (Create form)
  ├─ Edit.cshtml             (Edit form)
  ├─ Delete.cshtml           (Delete confirmation)
  └─ Details.cshtml          (View details)
```

## 📋 Features

✅ **CRUD Operations** - Create, Read, Update, Delete students  
✅ **Search & Filter** - Search students by name, email, phone  
✅ **Pagination** - 10 students per page with navigation  
✅ **Validation** - Server-side form validation  
✅ **Status Management** - Active, Inactive, Graduated  
✅ **Responsive UI** - Bootstrap 5 UI with FontAwesome icons  
✅ **Error Handling** - Comprehensive error management  
✅ **Logging** - Built-in ASP.NET logging  

## 🗄️ Database Setup

### 1. Create Database and Stored Procedures

Run the following SQL script to set up your database:

**Location:** `Data/StoredProcedures.sql`

**Using SQL Server Management Studio:**
```sql
-- Open SQL Server Management Studio
-- New Query
-- Open and run: Data/StoredProcedures.sql
```

**Using Command Line:**
```bash
sqlcmd -S (localdb)\mssqllocaldb -i "Data/StoredProcedures.sql"
```

### 2. Verify Setup

```sql
USE StudentManagementDb;

-- Check if table exists
SELECT * FROM Students;

-- Check if stored procedures are created
EXEC sp_GetAllStudents;
```

## 🚀 Running the Application

### Prerequisites
- .NET 8 or later
- SQL Server Express (LocalDB)
- Visual Studio or VS Code

### 1. Restore NuGet Packages
```bash
cd StudentManagementSystem/StudentManagementSystem
dotnet restore
```

### 2. Set Connection String (Optional)

Edit `appsettings.json` to change the database connection:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=StudentManagementDb;Trusted_Connection=true;"
  }
}
```

### 3. Run the Application
```bash
dotnet run
```

Navigate to `http://localhost:5000` in your browser.

## 📚 API Endpoints (MVC Routes)

| URL | Method | Action | Description |
|-----|--------|--------|-------------|
| `/Students` | GET | Index | List all students |
| `/Students?searchTerm=john&pageNumber=1` | GET | Index | Search and paginate |
| `/Students/Details/{id}` | GET | Details | View student details |
| `/Students/Create` | GET/POST | Create | Create new student |
| `/Students/Edit/{id}` | GET/POST | Edit | Edit student info |
| `/Students/Delete/{id}` | GET/POST | Delete | Delete student |

## 📦 Database Schema

### Students Table

```sql
CREATE TABLE Students (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PhoneNumber NVARCHAR(20) NOT NULL UNIQUE,
    DateOfBirth DATETIME NOT NULL,
    Address NVARCHAR(200) NOT NULL,
    EnrollmentDate DATETIME NOT NULL,
    Status NVARCHAR(20) NOT NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NOT NULL
);
```

## 🔧 Stored Procedures

### sp_GetAllStudents
Returns all students from the database.

### sp_GetStudentById
Returns a specific student by ID.

### sp_SearchStudents
Searches students by name, email, or phone number.

### sp_GetStudentsPaginated
Returns paginated results with optional search filter.

### sp_GetStudentCount
Returns the total count of students (or filtered count).

### sp_CreateStudent
Creates a new student record.

### sp_UpdateStudent
Updates an existing student record.

### sp_DeleteStudent
Deletes a student record.

### sp_GetActiveStudents
Returns only active students.

### sp_GetStudentsByStatus
Returns students filtered by status.

## 🎨 Views Overview

### Index View (Student List)
- Displays all students in a table format
- Search bar with search button
- Pagination controls
- Edit, View, Delete buttons for each student
- Add New Student button

### Create View
- Form fields for creating a new student
- Form validation with error messages
- Submit and Cancel buttons

### Edit View
- Pre-filled form with student data
- Form validation with error messages
- Update and Cancel buttons

### Delete View
- Confirmation dialog showing student details
- Warning message about permanent deletion
- Confirm Delete and Cancel buttons

### Details View
- Displays complete student information
- Status badge
- Calculated age display
- Edit, Delete, and Back buttons

## 🔒 Validation

### Model Validation
- **FirstName**: Required, Max 50 chars
- **LastName**: Required, Max 50 chars
- **Email**: Required, Valid email format, Unique
- **PhoneNumber**: Required, 10 digits, Unique
- **DateOfBirth**: Required, Past date only
- **Address**: Required, Max 200 chars
- **Status**: Required, Values: Active/Inactive/Graduated

### Business Logic Validation
- Duplicate email prevention
- Duplicate phone number prevention
- Age calculation from birthdate
- Data type validation

## 📊 Sample Data

To insert sample data into the database:

```sql
USE StudentManagementDb;

INSERT INTO Students (FirstName, LastName, Email, PhoneNumber, DateOfBirth, Address, Status)
VALUES 
    ('John', 'Doe', 'john.doe@example.com', '1234567890', '2005-03-15', '123 Main St, NYC', 'Active'),
    ('Jane', 'Smith', 'jane.smith@example.com', '0987654321', '2004-07-22', '456 Oak Ave, LA', 'Active'),
    ('Mike', 'Johnson', 'mike.j@example.com', '5551234567', '2006-01-10', '789 Pine Rd, Chicago', 'Inactive');
```

## 🧪 Testing

### Manual Testing Steps

1. **Create a Student**
   - Go to `/Students/Create`
   - Fill in all required fields
   - Click "Create Student"
   - Verify student appears in the list

2. **Search for Student**
   - Enter search term in search box
   - Click Search
   - Verify filtered results

3. **Edit a Student**
   - Click Edit button next to a student
   - Modify fields
   - Click "Update Student"
   - Verify changes

4. **Delete a Student**
   - Click Delete button
   - Confirm deletion
   - Verify student is removed

## 🐛 Troubleshooting

### Database Connection Issues
```
Error: Cannot open database
Fix: Ensure SQL Server LocalDB is running
Command: sqllocaldb start mssqllocaldb
```

### Stored Procedure Not Found
```
Error: Procedure not found
Fix: Run StoredProcedures.sql script to create them
```

### Port Already in Use
```
Error: localhost:5000 already in use
Fix: dotnet run --urls "https://localhost:5001;http://localhost:5001"
```

### Email/Phone Already Exists
```
Error: Duplicate entry
Fix: Use unique email and phone numbers
```

## 📝 Development Notes

### Adding New Features

1. **Add new field to Student model**
   - Update Models/Student.cs
   - Update database schema
   - Update stored procedures

2. **Create new view**
   - Create .cshtml file in Views/Students/
   - Add route in controller
   - Add link in navigation

3. **Add new service method**
   - Add interface in IStudentService
   - Implement in StudentService
   - Create stored procedure if needed

## 🔗 Integration with Frontend

The MVC app serves HTML views directly. To integrate with the Angular frontend:

1. Set up CORS in Program.cs:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", builder =>
    {
        builder
            .WithOrigins("http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});
app.UseCors("AllowAngular");
```

2. Modify StudentsController to return JSON for API calls:
```csharp
[ApiController]
[Route("api/[controller]")]
public class StudentsApiController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetStudents() { }
}
```

## 📞 Support

For issues or questions:
1. Check the troubleshooting section
2. Review error logs in console
3. Verify database connection string
4. Ensure stored procedures are created

## 📄 File Structure

```
StudentManagementSystem/
├── Controllers/
│   ├── HomeController.cs
│   └── StudentsController.cs
├── Models/
│   ├── Student.cs
│   └── ErrorViewModel.cs
├── Views/
│   ├── Students/
│   │   ├── Index.cshtml
│   │   ├── Create.cshtml
│   │   ├── Edit.cshtml
│   │   ├── Delete.cshtml
│   │   └── Details.cshtml
│   ├── Home/
│   └── Shared/
├── Services/
│   └── StudentService.cs
├── Data/
│   ├── IStudentRepository.cs
│   ├── StudentRepository.cs
│   └── StoredProcedures.sql
├── wwwroot/
├── Properties/
├── appsettings.json
├── Program.cs
└── StudentManagementSystem.csproj
```

## Next Steps

1. Run the application
2. Create sample students
3. Test all CRUD operations
4. Integrate with Angular frontend
5. Deploy to production

---

**Happy coding!** 🚀
