using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using OfficeFileAccessor.AppUsers.Entities;
using OfficeFileAccessor.OfficeFiles.Entities;

namespace OfficeFileAccessor.OfficeFiles.Records.Entities;

[Table("work_record")]
public class WorkRecord
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; init; }
    [Required]
    [Column("office_file_id")]
    public long OfficeFileId { get; init; }
    [Column("working_user_id")]
    public int? UpdateUserId { get; set; }
    [Column("finish_working_date", TypeName = "timestamp with time zone")]
    public DateTime? FinishWorkingDate { get; set; }
    [Required]
    [Column("last_update_date", TypeName = "timestamp with time zone")]
    public DateTime LastUpdateDate { get; set; }
    [JsonIgnore]
    public ApplicationUser? UpdateUser { get; set; }
    [JsonIgnore]
    public OfficeFile? OfficeFile { get; init; }
    public List<InputRecord> InputRecords { get; init; } = [];
}