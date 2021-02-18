using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsCollector.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsCollector.Data.Configurations
{
    public class KeywordConfiguration : IEntityTypeConfiguration<Keyword>
    {
        public void Configure(EntityTypeBuilder<Keyword> builder)
        {
            builder
               .HasKey(m => m.Id);

            builder
                .Property(m => m.Id)
                .UseIdentityColumn();

            builder
                .Property(m => m.KeywordValue)
                .HasMaxLength(100);

            builder
                .ToTable("Keywords");
        }
    }
}
