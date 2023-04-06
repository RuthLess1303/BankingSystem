using InternetBank.Db.Db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InternetBank.Db.Db.Mapping;

public class UserLoginsMap : IEntityTypeConfiguration<UserLoginsEntity>
{
    public void Configure(EntityTypeBuilder<UserLoginsEntity> builder)
    {
        builder.HasKey(l => l.Id);
        builder.Property(l => l.UserId).IsRequired();
        builder.Property(l => l.LoginDate).IsRequired();
    }
}