using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsCollector.Core.Models
{

    public class NewsKeyword
    {
        public int Id { get; set; }
        public int NewsId { get; set; }
        public News News { get; set; }
        public int KeywordId { get; set; }
        public Keyword Keyword { get; set; }
    } 
}

