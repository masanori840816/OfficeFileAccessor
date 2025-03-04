using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficeFileAccessor.OfficeFiles.Entities;

[Table("table_colmun_width")]
public record TableColumnWidth
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long? Id { get; init; }
    [Required]
    [Column("column")]
    public required int Column { get; init; } 
    [Required]
    [Column("width")]
    public required double Width { get; init; } 
    public List<OfficeFileSheet> OfficeFileSheets { get; set; } = [];
}