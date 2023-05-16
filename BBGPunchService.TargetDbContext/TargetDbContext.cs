using BBGPunchService.Core.Model.TargetEntity;
using Microsoft.EntityFrameworkCore;

namespace BBGPunchService.Source.Data
{
    public class TargetDbContext : DbContext
    {
        public TargetDbContext(DbContextOptions<TargetDbContext> options) : base(options) { }

        public DbSet<PunchingData> PunchingData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PunchingData>()
                .HasKey(p => p.Oid);
        }

    }

}
