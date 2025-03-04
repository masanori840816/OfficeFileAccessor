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
        modelBuilder.Entity<TableGroup>()
            .HasMany(g => g.TableCells)
            .WithMany(c => c.TableGroups)
            .UsingEntity<Dictionary<string, object>>(
            "TableGroupCell",
            j => j.HasOne<TableCell>()
                  .WithMany()
                  .HasForeignKey("table_cell_id")
                  .HasPrincipalKey(nameof(TableCell.Id)),
            j => j.HasOne<TableGroup>()
                  .WithMany()
                  .HasForeignKey("table_group_id")
                  .HasPrincipalKey(nameof(TableGroup.Id)),
            j =>
            {
                j.HasKey("table_cell_id", "table_group_id");
                j.ToTable("table_group_cell");
            });
    }
    public DbSet<ApplicationUser> ApplicationUsers => Set<ApplicationUser>();
    public DbSet<TableGroup> Groups => Set<TableGroup>();
    public DbSet<TableCell> Cells => Set<TableCell>();
    public DbSet<MergedTableCell> MergedCells => Set<MergedTableCell>();
    public DbSet<TableCellBorders> CellBorders => Set<TableCellBorders>();
    public DbSet<TableCellFontFormat> FontFormats => Set<TableCellFontFormat>();
}
