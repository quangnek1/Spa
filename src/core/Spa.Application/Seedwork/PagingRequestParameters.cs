namespace Spa.Application.Seedwork;

public class PagingRequestParameters
{
	private const int MaxPageSize = 50;

	private int _pageIndex = 1;
	public int PageIndex
	{
		get => _pageIndex;
		set => _pageIndex = value < 1 ? 1 : value;
	}

	private int _pageSize = 10;
	public int PageSize
	{
		get => _pageSize;
		set
		{
			if (value <= 0)
				_pageSize = 10;
			else
				_pageSize = value > MaxPageSize ? MaxPageSize : value;
		}
	}

	public string? SearchTerm { get; set; }
	public string? OrderBy { get; set; }
}
