namespace SOSYAL_KUTUPHANE_PLATFORMU.Models
{
  public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
    public int CurrentPage { get; set; }
        public int PageSize { get; set; }
   public int TotalPages { get; set; }
  public int TotalCount { get; set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;

      public PagedResult(List<T> items, int count, int pageNumber, int pageSize)
      {
  Items = items;
  TotalCount = count;
       CurrentPage = pageNumber;
       PageSize = pageSize;
  TotalPages = (int)Math.Ceiling(count / (double)pageSize);
      }
    }

    public class PaginationParams
    {
     private const int MaxPageSize = 50;
        private int _pageSize = 10;

   public int PageNumber { get; set; } = 1;

    public int PageSize
        {
            get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
      }
    }
}
