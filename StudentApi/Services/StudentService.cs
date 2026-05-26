using StudentApi.DTOs;
using StudentApi.Models;
using StudentApi.Repositories;

namespace StudentApi.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _repository;

    public StudentService(IStudentRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResultDto<StudentResponseDto>> GetAllAsync(
        int pageNumber,
        int pageSize,
        string? search,
        string? orderBy,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        var result = await _repository.GetAllAsync(pageNumber, pageSize, search, orderBy, cancellationToken);

        return new PagedResultDto<StudentResponseDto>
        {
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalRecords = result.TotalRecords,
            TotalPages = result.TotalPages,
            Data = result.Data.Select(ToResponseDto).ToList()
        };
    }

    public async Task<StudentResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var student = await _repository.GetByIdAsync(id, cancellationToken);
        return student is null ? null : ToResponseDto(student);
    }

    public async Task<StudentResponseDto> CreateAsync(StudentCreateDto dto, CancellationToken cancellationToken = default)
    {
        if (await _repository.EmailExistsAsync(dto.Email, null, cancellationToken))
        {
            throw new InvalidOperationException("Email already exists.");
        }

        if (await _repository.PhoneExistsAsync(dto.PhoneNumber, null, cancellationToken))
        {
            throw new InvalidOperationException("Phone number already exists.");
        }

        var student = new Student
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email,
            Password = dto.Password,
            CreatedDate = DateTime.UtcNow
        };

        var created = await _repository.CreateAsync(student, cancellationToken);
        return ToResponseDto(created);
    }

    public async Task<StudentResponseDto?> UpdateAsync(Guid id, StudentUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var student = await _repository.GetByIdAsync(id, cancellationToken);
        if (student is null)
        {
            return null;
        }

        if (await _repository.EmailExistsAsync(dto.Email, id, cancellationToken))
        {
            throw new InvalidOperationException("Email already exists.");
        }

        if (await _repository.PhoneExistsAsync(dto.PhoneNumber, id, cancellationToken))
        {
            throw new InvalidOperationException("Phone number already exists.");
        }

        student.Name = dto.Name;
        student.PhoneNumber = dto.PhoneNumber;
        student.Email = dto.Email;
        student.Password = dto.Password;

        await _repository.UpdateAsync(student, cancellationToken);
        return ToResponseDto(student);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _repository.DeleteAsync(id, cancellationToken);
    }

    private static StudentResponseDto ToResponseDto(Student student)
    {
        return new StudentResponseDto
        {
            Id = student.Id,
            Name = student.Name,
            PhoneNumber = student.PhoneNumber,
            Email = student.Email,
            CreatedDate = student.CreatedDate
        };
    }
}
