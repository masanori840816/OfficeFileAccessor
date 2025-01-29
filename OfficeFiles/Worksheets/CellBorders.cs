namespace OfficeFileAccessor.OfficeFiles.Worksheets;

public record CellBorders
{
    public BorderType Left { get; init; }
    public BorderType Top { get; init; }
    public BorderType Right { get; init; }
    public BorderType Bottom { get; init; }
    public override string ToString()
    {
        return $"Borders L:{Left} T:{Top} R:{Right} B:{Bottom}";
    }
    public static CellBorders GetNoBorders()
    {
        return new ()
        {
            Left = BorderType.None,
            Top = BorderType.None,
            Right = BorderType.None,
            Bottom = BorderType.None,
        };
    }
}