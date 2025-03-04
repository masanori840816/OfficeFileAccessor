using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficeFileAccessor.OfficeFiles.Entities;

[Table("office_file_sheet")]
public record OfficeFileSheet
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long? Id { get; init; }
    public required string Name { get; init; }
    public List<TableColumnWidth> ColumnWidths { get; init; } = [];
    public List<TableRowHeight> RowHeights { get; init; } = [];
    public List<TableGroup> TableGroups { get; init; } = [];
    public List<OfficeFile> OfficeFiles { get; init; } = [];
}