
using OfficeFileAccessor.OfficeFiles.Worksheets;

namespace OfficeFileAccessor.OfficeFiles.Files;

public record OfficeFileTableCell
{
    public long? Id { get; init; }
    public required CellAddress CellAddress { get; init; }
    public CellFontFormat? FontFormat { get; init; }
    public int VerticalLength { get; set; } = 1;
    public int HorizontalLength { get; set;} = 1;
    public required string Value { get; init; }
    public required CellBorders Borders { get; init; }
    public string? BackgroundColor { get; init; }
    public bool Editabled { get; init; }
    public MergedCell? MergedCell { get; init; }
    
    public required bool VerticalWriting { get; init; } = false;
    public required uint TextRotation { get; init; } = 0;
    public static OfficeFileTableCell Generate(Cell cell)
    {        
        return new ()
        {
            CellAddress = cell.Address,
            FontFormat = cell.FontFormat,
            HorizontalLength = 1,
            VerticalLength = 1,
            Value = cell.Value,
            Borders = cell.Borders,
            BackgroundColor = cell.BackgroundColor,
            Editabled = cell.BackgroundColor == ConstantParams.EditableColor,
            VerticalWriting = cell.VerticalWriting,
            TextRotation = cell.TextRotation,
        };
    }
    public static OfficeFileTableCell Generate(CellAddress baseAddress, string mergedValue, CellBorders borders,
        string? backgroundColor, MergedCell? mergedCell, bool verticalWriting, uint textRotation)
    {
        return new ()
        {
            CellAddress = baseAddress,
            Value = mergedValue,
            Borders = borders,
            BackgroundColor = backgroundColor,
            Editabled = backgroundColor == ConstantParams.EditableColor,
            MergedCell = mergedCell,
            VerticalWriting = verticalWriting,
            TextRotation = textRotation,
        };
    }
    public void UpdateCellLength(List<OfficeFileTableColumnWidth> widths, List<OfficeFileTableRowHeight> heights)
    {
        if(MergedCell == null)
        {
            return;
        }
        int horizontalLength = 0;
        int verticalLength = 0;
        foreach(OfficeFileTableColumnWidth w in widths)
        {
            if(MergedCell.Start.Column <= w.Column && MergedCell.End.Column >= w.Column)
            {
                horizontalLength += 1;
            }
        }
        foreach(OfficeFileTableRowHeight h in heights)
        {
            if(MergedCell.Start.Row <= h.Row && MergedCell.End.Row >= h.Row)
            {
                verticalLength += 1;
            }
        }
        HorizontalLength = horizontalLength;
        VerticalLength = verticalLength;
    }
}