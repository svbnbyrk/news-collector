using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsCollector.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsCollector.Data.Configurations
{
    public class SourceConfiguration : IEntityTypeConfiguration<Source>
    {
        public void Configure(EntityTypeBuilder<Source> builder)
        {
            builder
                .HasKey(m => m.Id);

            builder
                .Property(m => m.Id)
                .UseIdentityColumn();

            builder
                .Property(m => m.WebAdress);

            builder
                .Property(m => m.SourceName)
                .HasMaxLength(150);

            builder
                .ToTable("Sources");
        }
    }
}
