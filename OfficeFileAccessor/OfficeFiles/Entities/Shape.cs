using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OfficeFileAccessor.OfficeFiles.Entities;
[Table("shape")]
public class Shape
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long? Id { get; init; }
    [Required]
    [Column("office_file_sheet_id")]
    public long OfficeFileSheetId { get; init; }
    [Required]
    [Column("start_column")]
    public required int StartColumn { get; init; }
    [Required]
    [Column("start_row")]
    public required int StartRow { get; init; }
    [Required]
    [Column("start_offset_x")]
    public required double StartOffsetX { get; init; }
    [Required]
    [Column("start_offset_y")]
    public required double StartOffsetY { get; init; }

    [Column("end_column")]
    public int? EndColumn { get; set; }

    [Column("end_row")]
    public int? EndRow { get; set; }
    [Column("end_offset_x")]
    public double? EndOffsetX { get; set; }
    [Column("end_offset_y")]
    public double? EndOffsetY { get; set; }
    [Required]
    [Column("shape_type", TypeName = "varchar(64)")]
    [MaxLength(64)]
    public string ShapeType { get; set; } = "";
    [Required]
    [Column("value", TypeName = "varchar(512)")]
    [MaxLength(512)]
    public string Value { get; set; } = "";
    [JsonIgnore]
    public OfficeFileSheet? Sheet { get; init; }
}