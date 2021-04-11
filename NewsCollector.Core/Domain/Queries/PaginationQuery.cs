namespace NewsCollector.Core.Domain.Queries
{
    public class PaginationQuery
    {
        public PaginationQuery()
        {
            PageNumber = 1;
            PageSize = 100; 
        }
        public PaginationQuery(int pageNumber, int pageSize)
        {
            pageNumber = this.PageNumber;
            pageSize = this.PageSize;
        }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }

    }
}