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
            this.PageNumber= pageNumber; 
            this.PageSize = pageSize; 
        }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }

    }
}