using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NewsCollector.Core.Models
{
    public class Source
    {
        public int Id { get; set; }
        public string SourceName { get; set; }
        public string WebAdress { get; set; }
        public virtual ICollection<News> News { get; set; }
    }
}
