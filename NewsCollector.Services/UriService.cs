using System;
using System.Collections.Specialized;
using NewsCollector.Core.Domain.Queries;
using NewsCollector.Core.Helpers;
using NewsCollector.Core.Services;

namespace NewsCollector.Services
{
    public class UriService : IUriService
    {
        private readonly string _baseUri;
        public UriService(string baseUri)
        {
            this._baseUri = baseUri;
        }
        public Uri GetAllUri(PaginationQuery pagination = null)
        {
            QueryHelper queryHelper = new QueryHelper();
            var uri = new Uri(_baseUri);

            if (pagination == null)
            {
                return uri;
            }

            NameValueCollection query = new NameValueCollection();
            query.Add("pageNumber", pagination.PageNumber.ToString());
            query.Add("pageSize", pagination.PageSize.ToString());


            var modifiedUri = queryHelper.ToQueryString(_baseUri, query);

            return new Uri(modifiedUri);
        }
    }
}