using Microsoft.EntityFrameworkCore;

namespace DontMergeMeYet.Data
{
    public class InstallationContext : DbContext
    {
        public InstallationContext(DbContextOptions<InstallationContext> options)
            : base(options)
        {
        }

        public DbSet<RepositoryInstallation> RepositoryInstallations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var ri = modelBuilder.Entity<RepositoryInstallation>();
            ri.HasKey(r => r.RepositoryId);
            ri.Property(r => r.RepositoryId).ValueGeneratedNever();
            ri.HasIndex(r => r.InstallationId);
        }
    }
}
