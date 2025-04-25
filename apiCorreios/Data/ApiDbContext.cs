using apiCorreios.Models;
using Microsoft.EntityFrameworkCore;

namespace apiCorreios.Data
{
    public class ApiDbContext : DbContext
    {

        public DbSet<CorreiosToken> cToken { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder options)
           => options.UseSqlite("DataSource=Data/app.db;Cache=Shared");
    }
}
