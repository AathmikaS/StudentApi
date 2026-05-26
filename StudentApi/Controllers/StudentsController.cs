using Microsoft.AspNetCore.Mvc;
using StudentApi.DTOs;
using StudentApi.Services;


namespace StudentApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _service;

    public StudentsController(IStudentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<StudentResponseDto>>> GetAllStudents(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? orderBy = null,
        CancellationToken cancellationToken = default)
    {
        var students = await _service.GetAllAsync(pageNumber, pageSize, search, orderBy, cancellationToken);
        return Ok(students);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<StudentResponseDto>> GetStudentById(Guid id, CancellationToken cancellationToken)
    {
        var student = await _service.GetByIdAsync(id, cancellationToken);

        if (student is null)
        {
            return NotFound();
        }

        return Ok(student);
    }

    [HttpPost]
    public async Task<ActionResult<StudentResponseDto>> CreateStudent(
        StudentCreateDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var student = await _service.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetStudentById), new { id = student.Id }, student);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<StudentResponseDto>> UpdateStudent(
        Guid id,
        StudentUpdateDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var student = await _service.UpdateAsync(id, dto, cancellationToken);

            if (student is null)
            {
                return NotFound();
            }

            return Ok(student);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteStudent(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _service.DeleteAsync(id, cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}