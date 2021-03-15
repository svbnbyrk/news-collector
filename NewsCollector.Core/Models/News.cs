
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;
using static NewsCollector.Core.Enums.BaseEnums;

namespace NewsCollector.Core.Models
{
    public class News
    {
        public int Id { get; set; }
        public string NewsTitle { get; set; }
        public string NewsUrl { get; set; }
        public DateTime NewsDate { get; set; }
        public Languages Language { get; set; }
        public Source Source { get; set; }
        public int SourceId { get; set; }
        public string NewsContent { get; set; }             
        public NpgsqlTsVector SearchVector { get; set; }
        public virtual ICollection<NewsKeyword> NewsKeywords { get; set; }
    }
}
