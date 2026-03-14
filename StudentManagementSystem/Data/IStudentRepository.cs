using StudentManagementSystem.Models;

namespace StudentManagementSystem.Data
{
    public interface IStudentRepository
    {
        Task<IEnumerable<Student>> GetAllStudentsAsync();
        Task<Student?> GetStudentByIdAsync(int id);
        Task<IEnumerable<Student>> SearchStudentsAsync(string searchTerm);
        Task<IEnumerable<Student>> GetStudentsPaginatedAsync(int pageNumber, int pageSize, string? searchTerm = null);
        Task<int> GetTotalStudentCountAsync(string? searchTerm = null);
        Task<Student> CreateStudentAsync(Student student);
        Task<Student?> UpdateStudentAsync(int id, Student student);
        Task<bool> DeleteStudentAsync(int id);
        Task SaveChangesAsync();
    }
}
