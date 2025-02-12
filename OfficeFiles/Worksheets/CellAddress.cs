using OfficeFileAccessor.OfficeFiles.Worksheets.Functions;

namespace OfficeFileAccessor.OfficeFiles.Worksheets;

public record CellAddress
{
    public required string ColumnName { get; init; }
    public required int Column { get; init; }
    public required int Row { get; init; }

    /// <summary>
    /// Default: A1
    /// </summary>
    /// <returns></returns>
    public static CellAddress DefaultAddress()
    {
        return new () {
            ColumnName = "A",
            Column = 1,
            Row = 1,
        };
    }
    /// <summary>
    /// Move CellAddress
    /// </summary>
    /// <param name="original"></param>
    /// <param name="moveColumnLength">Moves the column left or right. Positive values ​​move right, negative values ​​move left</param>
    /// <param name="moveRowLength">Moves the row up or down. Positive values ​​move down, negative values ​​move up</param>
    /// <returns></returns>
    public static CellAddress Move(CellAddress original, int moveColumnLength, int moveRowLength)
    {
        int newColumn = original.Column + moveColumnLength;
        if(newColumn <= 0)
        {
            newColumn = 1;
        }
        int newRow = original.Row + moveRowLength;
        if(newRow <= 0)
        {
            newRow = 1;
        }
        return new ()
        {
            ColumnName = AddressConverter.ConvertIndexToAlphabet(newColumn),
            Column = newColumn,
            Row = newRow,
        };
    }
    /// <summary>
    /// Generate an instance from cell address like "A1".
    /// Return the default address when the argument is invalid.
    /// </summary>
    /// <param name="address"></param>
    /// <returns></returns>
    public static CellAddress GenerateFromAddress(string? address)
    {
        if(string.IsNullOrEmpty(address))
        {
            return DefaultAddress();
        }
        string columnName = AddressConverter.GetColumnNameFromAddress(address);
        return new () {
            ColumnName = columnName,
            Column = AddressConverter.ConvertAlphabetToIndex(columnName),
            Row = AddressConverter.GetRowFromAddress(address),
        };
    }
}