using StudentApi.DTOs;
using StudentApi.Models;

namespace StudentApi.Repositories;

public interface IStudentRepository
{
    Task<PagedResultDto<Student>> GetAllAsync(
        int pageNumber,
        int pageSize,
        string? search,
        string? orderBy,
        CancellationToken cancellationToken = default);

    Task<Student?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(string email, Guid? excludeId = null, CancellationToken cancellationToken = default);

    Task<bool> PhoneExistsAsync(string phoneNumber, Guid? excludeId = null, CancellationToken cancellationToken = default);

    Task<Student> CreateAsync(Student student, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(Student student, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
