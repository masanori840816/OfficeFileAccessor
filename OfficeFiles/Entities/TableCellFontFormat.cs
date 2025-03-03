using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficeFileAccessor.OfficeFiles.Entities;

[Table("table_cell_font_format")]
public record TableCellFontFormat
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long? Id { get; init; }
    [Column("table_cell_id")]
    public long TableCellId { get; init; }
    [Column("font_name", TypeName = "varchar(256)")]
    [MaxLength(256)]
    public string? FontName { get; init; }
    [Column("font_size")]
    public double? FontSize { get; init; }
    [Column("font_color")]
    public string? FontColor { get; init; }
    [Required]
    [Column("bold")]
    public required bool Bold { get; init; }
    public TableCell? TableCell { get; init; }
}