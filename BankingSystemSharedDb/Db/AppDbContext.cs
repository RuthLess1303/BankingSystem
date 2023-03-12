using BankingSystemSharedDb.Db.Entities;
using BankingSystemSharedDb.Db.Mapping;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InternetBankCore.Db;

public class AppDbContext : IdentityDbContext<UserEntity, RoleEntity, int>
{
    public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) : base(dbContextOptions)
    {
    }
    public DbSet<OperatorEntity> Operator { get; set; }
    public DbSet<UserEntity> User { get; set; }
    public DbSet<AccountEntity> Account { get; set; }
    public DbSet<CardEntity> Card { get; set; }
    public DbSet<CardAccountConnectionEntity> CardAccountConnection { get; set; }
    public DbSet<TransactionEntity> Transaction { get; set; }
    public DbSet<CurrencyEntity> Currency { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new AccountMap());
        builder.ApplyConfiguration(new CardMap());
        builder.ApplyConfiguration(new CurrencyMap());
        builder.ApplyConfiguration(new OperatorMap());
        builder.ApplyConfiguration(new TransactionMap());
        builder.ApplyConfiguration(new UserMap());
        builder.ApplyConfiguration(new CardAccountConnectionMap());
        
        base.OnModelCreating(builder);
        
        builder.Entity<RoleEntity>().HasData(new[]
        {
            new RoleEntity { Id = 1, Name = "user" },
            new RoleEntity { Id = 2, Name = "operator" }
        });
    }
}