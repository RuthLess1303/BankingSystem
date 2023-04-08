using InternetBank.Db.Db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InternetBank.Db.Db.Mapping;

public class LoggerMap : IEntityTypeConfiguration<LoggerEntity>
{
    public void Configure(EntityTypeBuilder<LoggerEntity> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.ProjectName).IsRequired();
        builder.Property(e => e.ThrowTime).IsRequired();
    }
}