using Microsoft.EntityFrameworkCore;
using OfficeFileAccessor.AppUsers.Entities;
using OfficeFileAccessor.OfficeFiles.Entities;

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
        modelBuilder.Entity<MergedTableCell>()
            .HasOne(m => m.TableCell)
            .WithOne(c => c.MergedCell)
            .HasForeignKey<MergedTableCell>(m => m.TableCellId)
            .IsRequired();
        modelBuilder.Entity<TableCellBorders>()
            .HasOne(m => m.TableCell)
            .WithOne(c => c.Borders)
            .HasForeignKey<TableCellBorders>(b => b.TableCellId)
            .IsRequired();
        modelBuilder.Entity<TableCellFontFormat>()
            .HasOne(m => m.TableCell)
            .WithOne(c => c.FontFormat)
            .HasForeignKey<TableCellFontFormat>(f => f.TableCellId)
            .IsRequired();
    }
    public DbSet<ApplicationUser> ApplicationUsers => Set<ApplicationUser>();
    public DbSet<TableCell> Cells => Set<TableCell>();
    public DbSet<MergedTableCell> MergedCells => Set<MergedTableCell>();
    public DbSet<TableCellBorders> CellBorders => Set<TableCellBorders>();
    public DbSet<TableCellFontFormat> FontFormats => Set<TableCellFontFormat>();
}
