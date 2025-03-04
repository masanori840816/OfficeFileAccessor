using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficeFileAccessor.OfficeFiles.Entities;

[Table("table_cell")]
public record TableCell
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long? Id { get; init; }
    [Required]
    [Column("column")]
    public required int Column { get; init; }
    [Required]
    [Column("row")]
    public required int Row { get; init; }
    [Required]
    [Column("vertical_length")]

    public int VerticalLength { get; init; } = 1;
    [Required]
    [Column("horizontal_length")]
    public int HorizontalLength { get; init;} = 1;
    [Required]
    [Column("value")]
    public required string Value { get; init; }
    
    [Column("background_color")]
    public string? BackgroundColor { get; init; }
    [Required]
    [Column("editabled")]
    public required bool Editabled { get; init; }    
    
    [Required]
    [Column("vertical_writing")]
    public required bool VerticalWriting { get; init; } = false;
    [Required]
    [Column("text_rotation")]
    public required int TextRotation { get; init; }
    public TableCellBorders? Borders { get; init; }
    public MergedTableCell? MergedCell { get; init; }
    public TableCellFontFormat? FontFormat { get; init; }
    public List<TableGroup> TableGroups { get; set; } = [];
}