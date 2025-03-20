namespace OfficeFileAccessor.OfficeFiles.Worksheets;

public record MergedCell
{
    public required int StartColumn { get; init; }
    public required int StartRow { get; init; }
    public required int EndColumn { get; init; }
    public required int EndRow { get; init; }

    public static MergedCell Generate(List<Cell> cells)
    {
        Cell[] ordered = [.. cells.OrderBy(c => c.Address.Column).ThenBy(c => c.Address.Row)];
        Cell firstCell = ordered.First();
        Cell lastCell = ordered.Last();
        return new ()
        {
            StartColumn = firstCell.Address.Column,
            StartRow = firstCell.Address.Row,
            EndColumn = lastCell.Address.Column,
            EndRow = lastCell.Address.Row,
        };
    }
}