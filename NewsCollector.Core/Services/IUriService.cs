using System;
using NewsCollector.Core.Domain.Queries;

namespace NewsCollector.Core.Services
{
    public interface IUriService
    {
        Uri GetAllUri(PaginationQuery pagination = null);
    }
}