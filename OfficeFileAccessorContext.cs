using Microsoft.EntityFrameworkCore;
using OfficeFileAccessor.AppUsers.Entities;

namespace OfficeFileAccessor;

public class OfficeFileAccessorContext(DbContextOptions<OfficeFileAccessorContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<ApplicationUser>()
                    .Property(w => w.LastUpdateDate)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
        modelBuilder.Entity<ApplicationUser>()
            .HasIndex(u => u.Email)
            .IsUnique();
        modelBuilder.Entity<ApplicationUser>()
            .HasData(ApplicationUser.Create("DefaultUser", "default@example.com", "oXc5rZbz", -1));
    }
    public DbSet<ApplicationUser> ApplicationUsers => Set<ApplicationUser>();
}
