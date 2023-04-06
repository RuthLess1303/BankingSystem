using InternetBank.Db.Db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InternetBank.Db.Db.Mapping;

public class LoginLoggerMap : IEntityTypeConfiguration<LoginLoggerEntity>
{
    public void Configure(EntityTypeBuilder<LoginLoggerEntity> builder)
    {
        builder.HasKey(l => l.Id);
        builder.Property(l => l.UserId).IsRequired();
        builder.Property(l => l.LoginDate).IsRequired();
    }
}