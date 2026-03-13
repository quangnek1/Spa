namespace Spa.Application.Seedwork;

public class PagedResult<T>
{
	public IEnumerable<T> Items { get; set; }
	public int TotalItems { get; set; }
	public int PageIndex { get; set; }
	public int PageSize { get; set; }
}
