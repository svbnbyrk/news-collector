using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsCollector.DTO
{
    public class NewsDTO
    {
        public int Id { get; set; }
        public string NewsTitle { get; set; }
        public string NewsUrl { get; set; }
        public DateTime NewsDate { get; set; }
        public string SourceName { get; set; }
        public int SourceId { get; set; }
    }
}
