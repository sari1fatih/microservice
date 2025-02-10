namespace Core.Application.Requests;

public class PageRequest
{
    public int PageIndex { get; set; }
    private int _pageSize;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > 20 ? 20 : value;
    }
}