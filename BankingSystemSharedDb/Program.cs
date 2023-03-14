using BankingSystemSharedDb.Db;
using BankingSystemSharedDb.Db.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
optionsBuilder.UseSqlServer("Server=localhost;Database=MobileBankDb;User Id=sa; Password=wavedi123;Encrypt=False;");