namespace OfficeFileAccessor.OfficeFiles.Files;

public record OfficeFile
{
    public long? Id { get; init; }
    
    public required string FileName { get; init; }
    public required string MimeType { get; init; }
    public List<OfficeFileSheet> Sheets { get; init; } = [];
    
}