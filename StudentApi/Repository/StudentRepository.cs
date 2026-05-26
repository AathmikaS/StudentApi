using Microsoft.EntityFrameworkCore;
using StudentApi.Data;
using StudentApi.DTOs;
using StudentApi.Models;

namespace StudentApi.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly AppDbContext _context;

    public StudentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResultDto<Student>> GetAllAsync(
        int pageNumber,
        int pageSize,
        string? search,
        string? orderBy,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Students.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();
            query = query.Where(s =>
                s.Name.ToLower().Contains(search) ||
                s.Email.ToLower().Contains(search) ||
                s.PhoneNumber.ToLower().Contains(search));
        }

        query = orderBy?.ToLower() switch
        {
            "name" => query.OrderBy(s => s.Name),
            "createddate" => query.OrderBy(s => s.CreatedDate),
            _ => query.OrderBy(s => s.Id)
        };

        var totalRecords = await query.CountAsync(cancellationToken);

        var data = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResultDto<Student>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
            Data = data
        };
    }

    public async Task<Student?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Students.FindAsync([id], cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        return await _context.Students.AnyAsync(s =>
            s.Email == email && (!excludeId.HasValue || s.Id != excludeId.Value), cancellationToken);
    }

    public async Task<bool> PhoneExistsAsync(string phoneNumber, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        return await _context.Students.AnyAsync(s =>
            s.PhoneNumber == phoneNumber && (!excludeId.HasValue || s.Id != excludeId.Value), cancellationToken);
    }

    public async Task<Student> CreateAsync(Student student, CancellationToken cancellationToken = default)
    {
        _context.Students.Add(student);
        await _context.SaveChangesAsync(cancellationToken);
        return student;
    }

    public async Task<bool> UpdateAsync(Student student, CancellationToken cancellationToken = default)
    {
        _context.Students.Update(student);
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var student = await GetByIdAsync(id, cancellationToken);
        if (student is null)
        {
            return false;
        }

        _context.Students.Remove(student);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
