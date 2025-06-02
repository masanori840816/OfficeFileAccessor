namespace OfficeFileAccessor.Files.DTO;

public record DownloadFile(string FileName, string MimeType, byte[] FileData);