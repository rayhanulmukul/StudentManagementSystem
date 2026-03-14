using StudentManagementSystem.Models;
using StudentManagementSystem.Data;

namespace StudentManagementSystem.Services
{
    public interface IStudentService
    {
        Task<IEnumerable<Student>> GetAllStudentsAsync();
        Task<Student?> GetStudentByIdAsync(int id);
        Task<IEnumerable<Student>> SearchStudentsAsync(string searchTerm);
        Task<IEnumerable<Student>> GetStudentsPaginatedAsync(int pageNumber, int pageSize, string? searchTerm = null);
        Task<int> GetTotalStudentCountAsync(string? searchTerm = null);
        Task<Student> CreateStudentAsync(Student student);
        Task<Student?> UpdateStudentAsync(int id, Student student);
        Task<bool> DeleteStudentAsync(int id);
    }

    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _repository;

        public StudentService(IStudentRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<Student>> GetAllStudentsAsync()
        {
            return _repository.GetAllStudentsAsync();
        }

        public Task<Student?> GetStudentByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid student ID", nameof(id));
            
            return _repository.GetStudentByIdAsync(id);
        }

        public Task<IEnumerable<Student>> SearchStudentsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return _repository.GetAllStudentsAsync();
            
            return _repository.SearchStudentsAsync(searchTerm);
        }

        public Task<IEnumerable<Student>> GetStudentsPaginatedAsync(int pageNumber, int pageSize, string? searchTerm = null)
        {
            if (pageNumber < 1 || pageSize < 1)
                throw new ArgumentException("Page number and page size must be greater than 0");
            
            return _repository.GetStudentsPaginatedAsync(pageNumber, pageSize, searchTerm);
        }

        public Task<int> GetTotalStudentCountAsync(string? searchTerm = null)
        {
            return _repository.GetTotalStudentCountAsync(searchTerm);
        }

        public Task<Student> CreateStudentAsync(Student student)
        {
            ValidateStudent(student);
            return _repository.CreateStudentAsync(student);
        }

        public Task<Student?> UpdateStudentAsync(int id, Student student)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid student ID", nameof(id));
            
            ValidateStudent(student);
            return _repository.UpdateStudentAsync(id, student);
        }

        public Task<bool> DeleteStudentAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid student ID", nameof(id));
            
            return _repository.DeleteStudentAsync(id);
        }

        private void ValidateStudent(Student student)
        {
            if (string.IsNullOrWhiteSpace(student.FirstName))
                throw new ArgumentException("First name is required");
            
            if (string.IsNullOrWhiteSpace(student.LastName))
                throw new ArgumentException("Last name is required");
            
            if (string.IsNullOrWhiteSpace(student.Email))
                throw new ArgumentException("Email is required");
            
            if (student.DateOfBirth >= DateTime.Now)
                throw new ArgumentException("Date of birth must be in the past");
        }
    }
}
