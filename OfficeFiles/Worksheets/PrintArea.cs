namespace OfficeFileAccessor.OfficeFiles.Worksheets;

public record PrintArea
{
    public required CellAddress Start { get; init; }
    public required CellAddress End { get; init; }

    public static PrintArea DefaultPrintArea()
    {
        return new ()
        {
            Start = new (ColumnName: "A", Column: 1, Row: 1),
            End = new (ColumnName: "IV", Column: 256, Row: 2000),
        };
    }
};