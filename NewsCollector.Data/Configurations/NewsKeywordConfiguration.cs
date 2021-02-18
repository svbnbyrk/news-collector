using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsCollector.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsCollector.Data.Configurations
{
    public class NewsKeywordConfiguration : IEntityTypeConfiguration<NewsKeyword>
    {
        public void Configure(EntityTypeBuilder<NewsKeyword> builder)
        {

            builder
                .HasKey(m => m.Id);

            builder.Property(m => m.Id)
                .UseIdentityColumn();

            builder.HasOne<News>(sc => sc.News)
                .WithMany(s => s.NewsKeywords)
                .HasForeignKey(sc => sc.NewsId);

            builder.HasOne<Keyword>(sc => sc.Keyword)
                .WithMany(s => s.NewsKeywords)
                .HasForeignKey(sc => sc.KeywordId);


            builder
                .ToTable("NewsKeywords");
        }
    }
}
