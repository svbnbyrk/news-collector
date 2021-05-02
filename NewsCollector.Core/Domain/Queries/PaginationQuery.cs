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
<<<<<<< HEAD
            this.PageNumber= pageNumber; 
            this.PageSize = pageSize; 
=======
            pageNumber = this.PageNumber;
            pageSize = this.PageSize;
>>>>>>> origin/development
        }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }

    }
}