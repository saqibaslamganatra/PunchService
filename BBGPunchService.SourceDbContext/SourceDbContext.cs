using BBGPunchService.Core.Model.SourceEntity;
using Microsoft.EntityFrameworkCore;

namespace BBGPunchService.Source.Data
{
    public class SourceDbContext : DbContext
    {
        public SourceDbContext(DbContextOptions<SourceDbContext> options) : base(options) { }

        public DbSet<Punch> Punch { get; set; }
        public DbSet<Employee> Employee { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Punch>()
                .HasKey(p => p.Oid);

            modelBuilder.Entity<Employee>()
                .HasKey(p => p.Oid);


            modelBuilder.Entity<Punch>()
                .HasOne(p => p.Employee)
                .WithMany(e => e.Punch)
                .HasForeignKey(p => p.EmployeeId);


        }
    }
}