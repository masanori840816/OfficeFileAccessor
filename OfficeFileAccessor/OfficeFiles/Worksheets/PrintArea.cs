namespace OfficeFileAccessor.OfficeFiles.Worksheets;

public record PrintArea
{
    public required int StartColumn { get; init; }
    public required int StartRow { get; init; }
    public required int EndColumn { get; init; }
    public required int EndRow { get; init; }

    public static PrintArea DefaultPrintArea()
    {
        return new ()
        {
            StartColumn = 1,
            StartRow = 1,
            EndColumn = 256,
            EndRow = 2000,
        };
    }
};