using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using OfficeFileAccessor.AppUsers.Entities;

namespace OfficeFileAccessor.OfficeFiles.Entities;

[Table("office_file")]
public record OfficeFile
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long? Id { get; init; }
    [Required]
    [Column("file_name", TypeName = "varchar(512)")]
    [MaxLength(512)]
    public required string FileName { get; init; }
    [Required]
    [Column("mime_type", TypeName = "varchar(512)")]
    [MaxLength(512)]
    public required string MimeType { get; init; }
    [Required]
    [Column("version")]
    public int Version { get; set; } = 1;
    [Required]
    [Column("register_user_id")]
    public required int RegisterUserId { get; init; }
    [Required]
    [Column("last_update_date", TypeName = "timestamp with time zone")]
    public required DateTime LastUpdateDate { get; set; }
    [JsonIgnore]
    public ApplicationUser? RegisterUser { get; init; }
    public OfficeFileData? OfficeFileData { get; init; }
    public List<OfficeFileSheet> OfficeFileSheets { get; init; } = [];
}