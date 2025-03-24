using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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
    [JsonIgnore]
    public TableCell? TableCell { get; init; }
    public static MergedTableCell Generate(List<Worksheets.Cell> cells)
    {
        Worksheets.Cell[] ordered = [.. cells.OrderBy(c => c.Address.Column).ThenBy(c => c.Address.Row)];
        Worksheets.Cell firstCell = ordered.First();
        Worksheets.Cell lastCell = ordered.Last();
        return new ()
        {
            StartColumn = firstCell.Address.Column,
            StartRow = firstCell.Address.Row,
            EndColumn = lastCell.Address.Column,
            EndRow = lastCell.Address.Row,
        };
    }
}