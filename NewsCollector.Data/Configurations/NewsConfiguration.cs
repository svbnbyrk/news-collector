using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsCollector.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsCollector.Data.Configurations
{
    public class NewsConfiguration : IEntityTypeConfiguration<News>
    {
        public void Configure(EntityTypeBuilder<News> builder)
        {
            builder
                .HasKey(m => m.Id);

            builder
                .Property(m => m.Id)
                .UseIdentityColumn();
           
            builder.HasOne(x => x.Source)
                .WithMany(y => y.News)
                .HasForeignKey(z => z.SourceId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder
                .ToTable("News");

        }
    }
}
