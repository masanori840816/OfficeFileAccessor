using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using OfficeFileAccessor.AppUsers.Entities;
using OfficeFileAccessor.OfficeFiles.Entities;

namespace OfficeFileAccessor.OfficeFiles.Records.Entities;

[Table("input_record")]
public class InputRecord
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; init; }
    [Required]
    [Column("work_record_id")]
    public long WorkRecordId { get; init; }
    [Required]
    [Column("table_cell_id")]
    public long TableCellId { get; init; }
    [Required]
    [Column("result")]
    public string Result { get; set; } = "";
    [Required]
    [Column("revision")]
    public required int Revision { get; init; }
    [Required]
    [Column("update_user_id")]
    public required int UpdateUserId { get; set; }
    [JsonIgnore]
    [Required]
    [Column("last_update_date", TypeName = "timestamp with time zone")]
    public DateTime LastUpdateDate { get; set; }
    [JsonIgnore]
    public WorkRecord? WorkRecord { get; init; }
    [JsonIgnore]
    public TableCell? TableCell { get; init; }
    [JsonIgnore]
    public ApplicationUser? UpdateUser { get; init; }
}