namespace OfficeFileAccessor.Apps;

public record ApplicationResult
{
    public bool Succeeded { get; init; }
    public string? ErrorMessage { get; init; }
    public static ApplicationResult GetSucceededResult()
    {
        return new ApplicationResult
        {
            Succeeded = true,
            ErrorMessage = null,
        };
    }
    public static ApplicationResult GetFailedResult(string errorMessage)
    {
        return new ApplicationResult
        {
            Succeeded = false,
            ErrorMessage = errorMessage,
        };
    }
}