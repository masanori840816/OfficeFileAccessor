using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OfficeFileAccessor.OfficeFiles.Worksheets;

namespace OfficeFileAccessor.OfficeFiles.Entities;

[Table("table_cell_borders")]
public record TableCellBorders
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long? Id { get; init; }
    [Column("table_cell_id")]
    public long TableCellId { get; init; }
    [Required]
    [Column("left")]
    public required int Left { get; init; }
    [Required]
    [Column("top")]
    public required int Top { get; init; }
    [Required]
    [Column("right")]
    public required int Right { get; init; }
    [Required]
    [Column("bottom")]
    public required int Bottom { get; init; }

    public TableCell? TableCell { get; init; }

    public static TableCellBorders GetNoBorders()
    {
        return new ()
        {
            Left = BorderType.None,
            Top = BorderType.None,
            Right = BorderType.None,
            Bottom = BorderType.None,
        };
    }
}