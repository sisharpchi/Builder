namespace Contract.Dto;

public class PaginationResponse<T>
{
    public PaginationResponse() : this(null, 0, 0)
    {
    }
    public PaginationResponse(IEnumerable<T>? items, int itemsCount, int pagesCount)
    {
        Items = items ?? Enumerable.Empty<T>();
        ItemsCount = itemsCount;
        PagesCount = pagesCount;
    }
    public IEnumerable<T> Items { get; init; } = null!;
    public int ItemsCount { get; init; }
    public int PagesCount { get; init; }
}
