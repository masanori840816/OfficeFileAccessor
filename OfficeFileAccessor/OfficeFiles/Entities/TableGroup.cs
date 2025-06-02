using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OfficeFileAccessor.OfficeFiles.Entities;

[Table("table_group")]
public class TableGroup
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long? Id { get; init; }
    [Required]
    [Column("display_order")]
    public required int DisplayOrder { get; set; }
    
    [Column("title", TypeName = "varchar(512)")]
    [MaxLength(512)]
    public string? Title { get; set; }
    [Column("bordered_group")]
    public required bool BorderedGroup { get; set; }
    public List<TableCell> TableCells { get; set; } = [];
    [JsonIgnore]
    public List<OfficeFileSheet> OfficeFileSheets { get; set; } = [];
}