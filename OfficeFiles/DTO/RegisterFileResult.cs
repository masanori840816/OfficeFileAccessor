using System.Text;
using System.Text.Json;
using OfficeFileAccessor.Files.DTO;
using OfficeFileAccessor.Apps;
using OfficeFileAccessor.OfficeFiles.Files;

namespace OfficeFileAccessor.OfficeFiles.DTO;

public record RegisterFileResult
{
    public required ApplicationResult Result { get; init; }
    public OfficeFile? File { get; init; }

    public DownloadFile GenerateDownloadFile(JsonSerializerOptions options)
    {
        string resultJson = JsonSerializer.Serialize(this, options);
        return new DownloadFile
        {
            FileName = "result.json",
            MimeType = "application/json",
            FileData = Encoding.UTF8.GetBytes(resultJson),
        };
    }
    public static DownloadFile GenerateFailedResult(string errorMessage, JsonSerializerOptions options)
    {
        RegisterFileResult result = new ()
        {
            Result = ApplicationResult.GetFailedResult(errorMessage),
        };
        return result.GenerateDownloadFile(options);
    }
}