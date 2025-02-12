namespace OfficeFileAccessor.Files.DTO;

public record DownloadFile
{
    public required string FileName { get; init; }
    public required string MimeType { get; init; }
    public required byte[] FileData { get; init; }

    
}