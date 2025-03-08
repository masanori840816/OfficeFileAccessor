using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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
    [JsonIgnore]
    public List<OfficeFileSheet> OfficeFileSheets { get; set; } = [];
}