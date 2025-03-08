using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OfficeFileAccessor.OfficeFiles.Entities;

[Table("table_cell")]
public class TableCell
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long? Id { get; init; }
    [Required]
    [Column("column")]
    public required int Column { get; init; }
    [Required]
    [Column("row")]
    public required int Row { get; init; }
    [Required]
    [Column("vertical_length")]

    public int VerticalLength { get; set; } = 1;
    [Required]
    [Column("horizontal_length")]
    public int HorizontalLength { get; set;} = 1;
    [Required]
    [Column("value")]
    public required string Value { get; init; }
    
    [Column("background_color")]
    public string? BackgroundColor { get; init; }
    [Required]
    [Column("editabled")]
    public required bool Editabled { get; init; }    
    
    [Required]
    [Column("vertical_writing")]
    public required bool VerticalWriting { get; init; } = false;
    [Required]
    [Column("text_rotation")]
    public required int TextRotation { get; init; }
    public TableCellBorders? Borders { get; init; }
    public MergedTableCell? MergedCell { get; init; }
    public TableCellFontFormat? FontFormat { get; init; }
    [JsonIgnore]
    public List<TableGroup> TableGroups { get; set; } = [];

    public static TableCell Generate(Worksheets.Cell cell, Worksheets.MergedCell? mergedCell,
        TableCellBorders borders)
    {
        MergedTableCell? merged = null;
        if(mergedCell != null)
        {
            merged = new ()
            {
                StartColumn = mergedCell.Start.Column,
                StartRow = mergedCell.Start.Row,
                EndColumn = mergedCell.End.Column, 
                EndRow = mergedCell.End.Row,
            };
        }
        return new ()
        {
            Column = cell.Address.Column,
            Row = cell.Address.Row,
            FontFormat = cell.FontFormat,
            HorizontalLength = 1,
            VerticalLength = 1,
            Value = cell.Value,
            Borders = borders,
            BackgroundColor = cell.BackgroundColor,
            Editabled = cell.BackgroundColor == ConstantParams.EditableColor,
            MergedCell = merged,
            VerticalWriting = cell.VerticalWriting,
            TextRotation = (int)cell.TextRotation,
        };
    }
    public static TableCell Generate(Worksheets.CellAddress baseAddress, string mergedValue, TableCellBorders borders,
        string? backgroundColor, Worksheets.MergedCell? mergedCell, bool verticalWriting, uint textRotation)
    {
        MergedTableCell? merged = null;
        if(mergedCell != null)
        {
            merged = new ()
            {
                StartColumn = mergedCell.Start.Column,
                StartRow = mergedCell.Start.Row,
                EndColumn = mergedCell.End.Column, 
                EndRow = mergedCell.End.Row,
            };
        }
        return new ()
        {
            Column = baseAddress.Column,
            Row = baseAddress.Row,
            Value = mergedValue,
            Borders = borders,
            BackgroundColor = backgroundColor,
            Editabled = backgroundColor == ConstantParams.EditableColor,
            MergedCell = merged,
            VerticalWriting = verticalWriting,
            TextRotation = (int)textRotation,
        };
    }
    public void UpdateCellLength(List<TableColumnWidth> widths, List<TableRowHeight> heights)
    {
        if(MergedCell == null)
        {
            return;
        }
        int horizontalLength = 0;
        int verticalLength = 0;
        foreach(TableColumnWidth w in widths)
        {
            if(MergedCell.StartColumn <= w.Column && MergedCell.EndColumn >= w.Column)
            {
                horizontalLength += 1;
            }
        }
        foreach(TableRowHeight h in heights)
        {
            if(MergedCell.StartRow <= h.Row && MergedCell.EndRow >= h.Row)
            {
                verticalLength += 1;
            }
        }
        HorizontalLength = horizontalLength;
        VerticalLength = verticalLength;
    }
}