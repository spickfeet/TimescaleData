using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebAPI.Database.Configurations;
using WebAPI.Database.Models;

namespace WebAPI.Database
{
    public class TimescaleDbContext : DbContext
    {
        public DbSet<ValueEntity> Values { get; set; }
        public DbSet<ResultEntity> Results { get; set; }
        public TimescaleDbContext(DbContextOptions<TimescaleDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ValueConfiguration());
            modelBuilder.ApplyConfiguration(new ResultConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
