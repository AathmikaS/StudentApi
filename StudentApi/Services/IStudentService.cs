using StudentApi.DTOs;

namespace StudentApi.Services;

public interface IStudentService
{
    Task<PagedResultDto<StudentResponseDto>> GetAllAsync(
        int pageNumber,
        int pageSize,
        string? search,
        string? orderBy,
        CancellationToken cancellationToken = default);

    Task<StudentResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<StudentResponseDto> CreateAsync(StudentCreateDto dto, CancellationToken cancellationToken = default);

    Task<StudentResponseDto?> UpdateAsync(Guid id, StudentUpdateDto dto, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
