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