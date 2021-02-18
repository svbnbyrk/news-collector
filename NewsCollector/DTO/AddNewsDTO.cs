using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsCollector.DTO
{
    public class AddNewsDTO
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public DateTime Date { get; set; }
    }
}
