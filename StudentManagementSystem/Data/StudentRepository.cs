using StudentManagementSystem.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentManagementSystem.Data
{
    public class StudentRepository : IStudentRepository
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;

        public StudentRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? "Server=(localdb)\\mssqllocaldb;Database=StudentManagementDb;Trusted_Connection=true;";
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public async Task<IEnumerable<Student>> GetAllStudentsAsync()
        {
            var students = new List<Student>();
            using (var connection = GetConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("sp_GetAllStudents", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            students.Add(new Student
                            {
                                Id = reader.GetInt32(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                Email = reader.GetString(3),
                                PhoneNumber = reader.GetString(4),
                                DateOfBirth = reader.GetDateTime(5),
                                Address = reader.GetString(6),
                                EnrollmentDate = reader.GetDateTime(7),
                                Status = reader.GetString(8)
                            });
                        }
                    }
                }
            }
            return students;
        }

        public async Task<Student?> GetStudentByIdAsync(int id)
        {
            using (var connection = GetConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("sp_GetStudentById", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@StudentId", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Student
                            {
                                Id = reader.GetInt32(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                Email = reader.GetString(3),
                                PhoneNumber = reader.GetString(4),
                                DateOfBirth = reader.GetDateTime(5),
                                Address = reader.GetString(6),
                                EnrollmentDate = reader.GetDateTime(7),
                                Status = reader.GetString(8)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task<IEnumerable<Student>> SearchStudentsAsync(string searchTerm)
        {
            var students = new List<Student>();
            using (var connection = GetConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("sp_SearchStudents", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@SearchTerm", searchTerm ?? "");
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            students.Add(new Student
                            {
                                Id = reader.GetInt32(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                Email = reader.GetString(3),
                                PhoneNumber = reader.GetString(4),
                                DateOfBirth = reader.GetDateTime(5),
                                Address = reader.GetString(6),
                                EnrollmentDate = reader.GetDateTime(7),
                                Status = reader.GetString(8)
                            });
                        }
                    }
                }
            }
            return students;
        }

        public async Task<IEnumerable<Student>> GetStudentsPaginatedAsync(int pageNumber, int pageSize, string? searchTerm = null)
        {
            var students = new List<Student>();
            using (var connection = GetConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("sp_GetStudentsPaginated", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PageNumber", pageNumber);
                    command.Parameters.AddWithValue("@PageSize", pageSize);
                    command.Parameters.AddWithValue("@SearchTerm", searchTerm ?? "");
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            students.Add(new Student
                            {
                                Id = reader.GetInt32(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                Email = reader.GetString(3),
                                PhoneNumber = reader.GetString(4),
                                DateOfBirth = reader.GetDateTime(5),
                                Address = reader.GetString(6),
                                EnrollmentDate = reader.GetDateTime(7),
                                Status = reader.GetString(8)
                            });
                        }
                    }
                }
            }
            return students;
        }

        public async Task<int> GetTotalStudentCountAsync(string? searchTerm = null)
        {
            using (var connection = GetConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("sp_GetStudentCount", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@SearchTerm", searchTerm ?? "");
                    var result = await command.ExecuteScalarAsync();
                    return result != null ? (int)result : 0;
                }
            }
        }

        public async Task<Student> CreateStudentAsync(Student student)
        {
            using (var connection = GetConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("sp_CreateStudent", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@FirstName", student.FirstName);
                    command.Parameters.AddWithValue("@LastName", student.LastName);
                    command.Parameters.AddWithValue("@Email", student.Email);
                    command.Parameters.AddWithValue("@PhoneNumber", student.PhoneNumber);
                    command.Parameters.AddWithValue("@DateOfBirth", student.DateOfBirth);
                    command.Parameters.AddWithValue("@Address", student.Address);
                    command.Parameters.AddWithValue("@Status", student.Status);

                    var result = await command.ExecuteScalarAsync();
                    if (result != null)
                    {
                        student.Id = (int)result;
                    }
                }
            }
            return student;
        }

        public async Task<Student?> UpdateStudentAsync(int id, Student student)
        {
            using (var connection = GetConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("sp_UpdateStudent", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@StudentId", id);
                    command.Parameters.AddWithValue("@FirstName", student.FirstName);
                    command.Parameters.AddWithValue("@LastName", student.LastName);
                    command.Parameters.AddWithValue("@Email", student.Email);
                    command.Parameters.AddWithValue("@PhoneNumber", student.PhoneNumber);
                    command.Parameters.AddWithValue("@DateOfBirth", student.DateOfBirth);
                    command.Parameters.AddWithValue("@Address", student.Address);
                    command.Parameters.AddWithValue("@Status", student.Status);

                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected > 0)
                    {
                        student.Id = id;
                        return student;
                    }
                }
            }
            return null;
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            using (var connection = GetConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("sp_DeleteStudent", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@StudentId", id);
                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }

        public async Task SaveChangesAsync()
        {
            // Not needed for stored procedure approach, but kept for interface compliance
            await Task.CompletedTask;
        }
    }
}
