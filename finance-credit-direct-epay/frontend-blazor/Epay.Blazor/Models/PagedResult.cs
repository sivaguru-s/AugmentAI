namespace Epay.Blazor.Models;

/// <summary>
/// Generic paged result wrapper (mirrors backend DTO)
/// </summary>
/// <typeparam name="T">Type of items</typeparam>
public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalRecords { get; set; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
