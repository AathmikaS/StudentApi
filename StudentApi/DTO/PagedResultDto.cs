namespace StudentApi.DTOs;

public class PagedResultDto<T>
{
    public int TotalRecords { get; set; }
    public int TotalPages { get; set; }
    public List<T> Data { get; set; } = new();

    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}