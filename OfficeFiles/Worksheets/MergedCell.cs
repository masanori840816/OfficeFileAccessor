namespace OfficeFileAccessor.OfficeFiles.Worksheets;

public record MergedCell
{
    public required CellAddress Start { get; init; }
    public required CellAddress End { get; init; }

    public static MergedCell Generate(List<Cell> cells)
    {
        Cell[] ordered = [.. cells.OrderBy(c => c.Address.Column).ThenBy(c => c.Address.Row)];
        Cell firstCell = ordered.First();
        Cell lastCell = ordered.Last();
        return new ()
        {
            Start = firstCell.Address,
            End = lastCell.Address,
        };
    }
}