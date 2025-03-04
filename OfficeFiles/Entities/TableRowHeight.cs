using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficeFileAccessor.OfficeFiles.Entities;

[Table("table_row_height")]
public record TableRowHeight
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long? Id { get; init; }
    [Required]
    [Column("row")]
    public required int Row { get; init; }
    [Required]
    [Column("height")]
    public required double Height { get; init; }
    public List<OfficeFileSheet> OfficeFileSheets { get; set; } = [];
}