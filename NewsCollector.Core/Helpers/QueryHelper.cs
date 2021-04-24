using System.Linq;
using System.Web;
using System.Collections.Specialized;

namespace NewsCollector.Core.Helpers
{
    public class QueryHelper
    {
        public string ToQueryString( string url ,NameValueCollection nvc)
        {
            var array = (
                from key in nvc.AllKeys
                from value in nvc.GetValues(key)
                select string.Format(
                "{0}={1}",
                HttpUtility.UrlEncode(key),
                HttpUtility.UrlEncode(value))
                ).ToArray();
                   
            return url + "?" + string.Join("&", array);
        }

    }
}