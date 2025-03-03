using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficeFileAccessor.OfficeFiles.Entities;

[Table("merged_table_cell")]
public record MergedTableCell
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long? Id { get; init; }
    [Required]
    [Column("table_cell_id")]
    public long TableCellId { get; init; }
    [Required]
    [Column("start_column")]
    public required int StartColumn { get; init; }
    [Required]
    [Column("start_row")]
    public required int StartRow { get; init; }
    [Required]
    [Column("end_column")]
    public required int EndColumn { get; init; }
    [Required]
    [Column("end_row")]
    public required int EndRow { get; init; }

    public TableCell? TableCell { get; init; }
}