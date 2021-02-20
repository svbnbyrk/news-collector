using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace NewsCollector.Core.Models
{
    public class Keyword
    {
        public Keyword(){
            NewsKeywords = new Collection<NewsKeyword>();
        }

        public int Id { get; set; }

        public string KeywordValue { get; set; }

        public virtual ICollection<NewsKeyword> NewsKeywords { get; set; }
    }
}
