using System;
using System.Collections.Generic;
using System.Text;

namespace NewsCollector.Core.Models
{
    public class Keyword
    {
        public int Id { get; set; }

        public string KeywordValue { get; set; }

        public virtual ICollection<NewsKeyword> NewsKeywords { get; set; }
    }
}
