namespace BlueSelfCheckout.WebApi.Models
{
    

    public class PagedResponse<T>
    {
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public List<T> Data { get; set; }

        public PagedResponse(int totalCount, int pageNumber, int pageSize, List<T> data)
        {
            TotalCount = totalCount;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            Data = data;
        }
    }

}
