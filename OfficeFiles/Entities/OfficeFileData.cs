using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OfficeFileAccessor.OfficeFiles.Entities;

[Table("office_file_data")]
public record OfficeFileData
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long? Id { get; init; }
    [Required]
    [Column("file_id")]
    public long FileId { get; init; }
    [Required]
    [Column("file_data")]
    public required byte[] FileData { get; init; }
    [JsonIgnore]
    public OfficeFile? OfficeFile { get; init; }    
}