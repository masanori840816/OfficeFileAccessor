using Microsoft.EntityFrameworkCore;
using OfficeFileAccessor.AppUsers.DTO;
using OfficeFileAccessor.AppUsers.Entities;
using OfficeFileAccessor.OfficeFiles.DTO;
using OfficeFileAccessor.OfficeFiles.Entities;
using OfficeFileAccessor.OfficeFiles.Records.Entities;

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
            "LinkGroupCell",
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
                j.ToTable("link_group_cell");
            });
        modelBuilder.Entity<OfficeFileSheet>()
            .HasMany(s => s.TableGroups)
            .WithMany(g => g.OfficeFileSheets)
            .UsingEntity<Dictionary<string, object>>(
            "LinkSheetGroup",
            j => j.HasOne<TableGroup>()
                  .WithMany()
                  .HasForeignKey("table_group_id")
                  .HasPrincipalKey(nameof(TableGroup.Id)),
            j => j.HasOne<OfficeFileSheet>()
                  .WithMany()
                  .HasForeignKey("sheet_id")
                  .HasPrincipalKey(nameof(OfficeFileSheet.Id)),
            j =>
            {
                j.HasKey("table_group_id", "sheet_id");
                j.ToTable("link_sheet_group");
            });
        modelBuilder.Entity<OfficeFileSheet>()
            .HasMany(s => s.ColumnWidths)
            .WithMany(g => g.OfficeFileSheets)
            .UsingEntity<Dictionary<string, object>>(
            "LinkSheetColumnWidth",
            j => j.HasOne<TableColumnWidth>()
                  .WithMany()
                  .HasForeignKey("column_width_id")
                  .HasPrincipalKey(nameof(TableColumnWidth.Id)),
            j => j.HasOne<OfficeFileSheet>()
                  .WithMany()
                  .HasForeignKey("sheet_id")
                  .HasPrincipalKey(nameof(OfficeFileSheet.Id)),
            j =>
            {
                j.HasKey("column_width_id", "sheet_id");
                j.ToTable("link_sheet_column_width");
            });
        modelBuilder.Entity<OfficeFileSheet>()
            .HasMany(s => s.RowHeights)
            .WithMany(g => g.OfficeFileSheets)
            .UsingEntity<Dictionary<string, object>>(
            "LinkSheetRowHeight",
            j => j.HasOne<TableRowHeight>()
                  .WithMany()
                  .HasForeignKey("row_height_id")
                  .HasPrincipalKey(nameof(TableRowHeight.Id)),
            j => j.HasOne<OfficeFileSheet>()
                  .WithMany()
                  .HasForeignKey("sheet_id")
                  .HasPrincipalKey(nameof(OfficeFileSheet.Id)),
            j =>
            {
                j.HasKey("row_height_id", "sheet_id");
                j.ToTable("link_sheet_row_height");
            });
        modelBuilder.Entity<OfficeFile>()
            .HasMany(s => s.OfficeFileSheets)
            .WithMany(g => g.OfficeFiles)
            .UsingEntity<Dictionary<string, object>>(
            "LinkFileSheet",
            j => j.HasOne<OfficeFileSheet>()
                  .WithMany()
                  .HasForeignKey("sheet_id")
                  .HasPrincipalKey(nameof(OfficeFileSheet.Id)),
            j => j.HasOne<OfficeFile>()
                  .WithMany()
                  .HasForeignKey("file_id")
                  .HasPrincipalKey(nameof(OfficeFile.Id)),
            j =>
            {
                j.HasKey("sheet_id", "file_id");
                j.ToTable("link_file_sheet");
            });        
        modelBuilder.Entity<OfficeFile>()
                    .HasOne(i => i.RegisterUser)
                    .WithMany(u => u.OfficeFiles)
                    .HasForeignKey(w => w.RegisterUserId)
                    .HasConstraintName("FK_OfficeFile_ApplicationUser");

        modelBuilder.Entity<OfficeFileData>()
            .HasOne(d => d.OfficeFile)
            .WithOne(f => f.OfficeFileData)
            .HasForeignKey<OfficeFileData>(f => f.FileId)
            .IsRequired();
        modelBuilder.Entity<Shape>()
            .HasOne(s => s.Sheet)
            .WithMany(s => s.Shapes)
            .HasForeignKey(s => s.OfficeFileSheetId)
            .HasConstraintName("FK_Shape_OfficeFileSheet");

        modelBuilder.Entity<WorkRecord>()
                    .HasOne(i => i.OfficeFile)
                    .WithMany(u => u.WorkRecords)
                    .HasForeignKey(w => w.OfficeFileId)
                    .HasConstraintName("FK_WorkRecord_OfficeFile");        
        modelBuilder.Entity<WorkRecord>()
                    .HasOne(i => i.UpdateUser)
                    .WithMany(u => u.WorkRecords)
                    .HasForeignKey(w => w.UpdateUserId)
                    .HasConstraintName("FK_WorkRecord_ApplicationUser");

        modelBuilder.Entity<InputRecord>()
                    .HasOne(i => i.WorkRecord)
                    .WithMany(u => u.InputRecords)
                    .HasForeignKey(w => w.WorkRecordId)
                    .HasConstraintName("FK_InputRecord_WorkRecord");        
        modelBuilder.Entity<InputRecord>()
                    .HasOne(i => i.TableCell)
                    .WithMany(u => u.InputRecords)
                    .HasForeignKey(w => w.TableCellId)
                    .HasConstraintName("FK_InputRecord_TableCell");
        modelBuilder.Entity<InputRecord>()
                    .HasOne(i => i.UpdateUser)
                    .WithMany(u => u.InputRecords)
                    .HasForeignKey(w => w.UpdateUserId)
                    .HasConstraintName("FK_InputRecord_ApplicationUser");
        
        /* exclude from migration */
        modelBuilder.Entity<PreviewOfficeFileSheets>().HasNoKey().ToView(null);
        modelBuilder.Entity<DisplayOfficeFileCell>().HasNoKey().ToView(null);
        modelBuilder.Entity<SearchUser>().HasNoKey().ToView(null);
        modelBuilder.Entity<SearchOfficeFile>().HasNoKey().ToView(null);
    }
    public DbSet<ApplicationUser> ApplicationUsers => Set<ApplicationUser>();
    public DbSet<OfficeFile> OfficeFiles => Set<OfficeFile>();
    public DbSet<OfficeFileData> OfficeFileData => Set<OfficeFileData>();
    public DbSet<OfficeFileSheet> Sheets => Set<OfficeFileSheet>();
    
    public DbSet<TableColumnWidth> ColumnWidths => Set<TableColumnWidth>();
    public DbSet<TableRowHeight> RowHeights => Set<TableRowHeight>();
    public DbSet<TableGroup> Groups => Set<TableGroup>();
    public DbSet<TableCell> Cells => Set<TableCell>();
    public DbSet<MergedTableCell> MergedCells => Set<MergedTableCell>();
    public DbSet<TableCellBorders> CellBorders => Set<TableCellBorders>();
    public DbSet<TableCellFontFormat> FontFormats => Set<TableCellFontFormat>();

    public DbSet<WorkRecord> WorkRecords => Set<WorkRecord>();
    public DbSet<InputRecord> InputRecords => Set<InputRecord>();
    /* -- only for displaying */
    public DbSet<PreviewOfficeFileSheets> PreviewSheets => Set<PreviewOfficeFileSheets>();
    public DbSet<DisplayOfficeFileCell> DisplayCells => Set<DisplayOfficeFileCell>();
    public DbSet<SearchUser> SearchUsers => Set<SearchUser>();
    public DbSet<SearchOfficeFile> SearchOfficeFiles => Set<SearchOfficeFile>();
}
