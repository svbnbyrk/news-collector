using System.Collections.Generic;
using System.Linq;
using NewsCollector.Core.Domain;
using NewsCollector.Core.Domain.Queries;
using NewsCollector.Core.Domain.Responses;
using NewsCollector.Core.Services;

namespace NewsCollector.Helpers
{
    public class PaginationHelpers
    {
        public static PagedResponse<T> CreatePaginationResponse<T>(IUriService uriService,
                                                         PaginationFilter pagination,
                                                         IEnumerable<T> response)
        {
            var nextPage = pagination.PageNumber >= 1 ? uriService.GetAllUri(new PaginationQuery(pagination.PageNumber + 1, pagination.PageSize)).ToString() : null;
            var prevPage = pagination.PageNumber - 1 >= 1 ? uriService.GetAllUri(new PaginationQuery(pagination.PageNumber - 1, pagination.PageSize)).ToString() : null;

            return new PagedResponse<T>
            {
                Data = response,
                NextPage = response.Any() ? nextPage.ToString() : null,
                PrevPage = prevPage?.ToString(),
                PageSize = pagination.PageSize >= 1 ? pagination.PageSize : (int?)null,
                PageNumber = pagination.PageNumber >= 1 ? pagination.PageNumber : (int?)null
            };
        }

    }

}