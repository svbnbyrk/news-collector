using Microsoft.EntityFrameworkCore;
using NewsCollector.Core.Models;
using NewsCollector.Data.Configurations;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsCollector.Data
{
    public class NewsCollectorDbContext : DbContext
    {
        public DbSet<Keyword> Keywords { get; set; }
        public DbSet<Source> Sources { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<NewsKeyword> NewsKeywords { get; set; }

        public NewsCollectorDbContext(DbContextOptions<NewsCollectorDbContext> options) : base(options)
        {

        }



        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{

        //}

        protected override void OnModelCreating(ModelBuilder builder)
        {

            builder.ApplyConfiguration(new NewsConfiguration());
            builder.ApplyConfiguration(new KeywordConfiguration());
            builder.ApplyConfiguration(new NewsKeywordConfiguration());
            builder.ApplyConfiguration(new SourceConfiguration());

        }       
    }
}
