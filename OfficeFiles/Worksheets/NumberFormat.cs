namespace OfficeFileAccessor.OfficeFiles.Worksheets;

public record NumberFormat
{
    public required uint NumberFormatId { get; init; }
    public required string ValueType { get; init; }
    public required string Format { get; init; }

    public static NumberFormat? DefaultNumberFormat(uint numberFormatId)
    {
        switch(numberFormatId)
        {
            case 2:
                // 0.00
                return new NumberFormat() { NumberFormatId = numberFormatId, 
                    ValueType = CellValueType.Double,
                    Format = "0.00" };
            case 9:
                // %
                return new NumberFormat() { NumberFormatId = numberFormatId,
                    ValueType = CellValueType.Double,
                    Format = "P1" };
            case 38:
                // 0
                return new NumberFormat() { NumberFormatId = numberFormatId,
                    ValueType = CellValueType.Integer,
                    Format = "0" };
            case 40:
                // 0.00
                return new NumberFormat() { NumberFormatId = numberFormatId,
                    ValueType = CellValueType.Double,
                    Format = "0.00" };
            case 49:
                return new NumberFormat() { NumberFormatId = numberFormatId,
                    ValueType = CellValueType.Text,
                    Format = "" };
            default:
                return null;
        }
    }
}